namespace SmsSenderClient.Services;

public interface ISignalSender
{
    Task SendTextAsync(string to, string text, CancellationToken ct = default);
}