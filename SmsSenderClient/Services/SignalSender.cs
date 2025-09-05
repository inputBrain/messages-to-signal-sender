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

    public async Task SendTextAsync(string[] to, string text, CancellationToken ct = default)
    {
        var payload = new
        {
            message = text,
            number = _cfg.SendMessageFrom,
            recipients = to
        };

        using var res = await _http.PostAsJsonAsync("v2/send", payload, ct);
        res.EnsureSuccessStatusCode();
    }
    
    
    public async Task SendAsync(string[] to, string text, IEnumerable<string>? base64Attachments = null, CancellationToken ct = default)
    {
        var payload = new
        {
            message = text,
            number = _cfg.SendMessageFrom,
            recipients = to,
            base64_attachments = base64Attachments
        };

        using var res = await _http.PostAsJsonAsync("v2/send", payload, ct);
        res.EnsureSuccessStatusCode();
    }
}