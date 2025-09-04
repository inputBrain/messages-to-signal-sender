using Microsoft.Extensions.Options;
using SmsSenderClient.Configs;

namespace SmsSenderClient.Services;

public sealed class SignalSender : ISignalSender
{
    private readonly HttpClient _http;
    private readonly SignalSettings _cfg;

    public SignalSender(HttpClient http, IOptions<SignalSettings> cfg)
    {
        _http = http;
        _cfg = cfg.Value;
        _http.BaseAddress = new Uri(_cfg.BaseUrl.TrimEnd('/') + "/");
    }

    public async Task SendTextAsync(string to, string text, CancellationToken ct = default)
    {
        var payload = new
        {
            message = text,
            number = _cfg.SendMessageFrom,
            recipients = new[] { to }
        };

        using var res = await _http.PostAsJsonAsync("v2/send", payload, ct);
        res.EnsureSuccessStatusCode();
    }
}