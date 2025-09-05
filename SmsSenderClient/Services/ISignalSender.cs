namespace SmsSenderClient.Services;

public interface ISignalSender
{
    Task SendTextAsync(string[] to, string text, CancellationToken ct = default);
    Task SendAsync(string[] to, string text, IEnumerable<string>? base64Attachments = null, CancellationToken ct = default);
}