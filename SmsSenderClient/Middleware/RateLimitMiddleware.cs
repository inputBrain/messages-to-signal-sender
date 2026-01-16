using System.Collections.Concurrent;

namespace SmsSenderClient.Middleware;

public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitMiddleware> _logger;


    private static readonly ConcurrentDictionary<string, List<DateTime>> RequestLog = new();

    private const int MaxRequestsPerMinute = 10;
    private const int CleanupIntervalMinutes = 5;

    private static DateTime _lastCleanup = DateTime.UtcNow;

    public RateLimitMiddleware(RequestDelegate next, ILogger<RateLimitMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/Message/SendMessage") && context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
        {
            var clientIp = GetClientIp(context);

            if (IsRateLimited(clientIp))
            {
                _logger.LogWarning("Rate limit exceeded for IP: {IP}", clientIp);

                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync(
                    "<h3>Забагато запитів</h3>" +
                    "<p>Будь ласка, зачекайте хвилину перед наступною спробою.</p>");
                return;
            }

            RecordRequest(clientIp);
        }

        CleanupOldEntries();

        await _next(context);
    }

    private static string GetClientIp(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private static bool IsRateLimited(string clientIp)
    {
        if (!RequestLog.TryGetValue(clientIp, out var timestamps))
        {
            return false;
        }

        var oneMinuteAgo = DateTime.UtcNow.AddMinutes(-1);
        var recentRequests = timestamps.Count(t => t > oneMinuteAgo);

        return recentRequests >= MaxRequestsPerMinute;
    }

    private static void RecordRequest(string clientIp)
    {
        var timestamps = RequestLog.GetOrAdd(clientIp, _ => new List<DateTime>());

        lock (timestamps)
        {
            timestamps.Add(DateTime.UtcNow);

            var oneMinuteAgo = DateTime.UtcNow.AddMinutes(-1);
            timestamps.RemoveAll(t => t < oneMinuteAgo);
        }
    }

    private static void CleanupOldEntries()
    {
        if ((DateTime.UtcNow - _lastCleanup).TotalMinutes < CleanupIntervalMinutes)
        {
            return;
        }

        _lastCleanup = DateTime.UtcNow;
        var threshold = DateTime.UtcNow.AddMinutes(-CleanupIntervalMinutes);

        foreach (var key in RequestLog.Keys.ToList())
        {
            if (RequestLog.TryGetValue(key, out var timestamps))
            {
                lock (timestamps)
                {
                    timestamps.RemoveAll(t => t < threshold);
                    if (timestamps.Count == 0)
                    {
                        RequestLog.TryRemove(key, out _);
                    }
                }
            }
        }
    }
}

public static class RateLimitMiddlewareExtensions
{
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RateLimitMiddleware>();
    }
}
