using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmsSenderClient.Configs;
using SmsSenderClient.Database.Message;
using SmsSenderClient.Models;
using SmsSenderClient.Services;

namespace SmsSenderClient.Controllers;

public class MessageController : Controller
{
    private readonly ILogger<MessageController> _logger;
    private readonly IMessageRepository _messageRepository;
    private readonly ISignalSender _signal;

    private readonly SignalSettings _signalSettings;

    public MessageController(ILogger<MessageController> logger, IMessageRepository messageRepository, ISignalSender signal, IOptions<SignalSettings> signalSettings)
    {
        _logger = logger;
        _messageRepository = messageRepository;
        _signal = signal;
        _signalSettings = signalSettings.Value;
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendMessage(MessageViewModel vm, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending message");
        _logger.LogInformation("Name: {VmName} | Message: {VmMessage}", vm.Name, vm.Message);
        
        const long MAX_SIZE = 5 * 1024 * 1024;
        string? base64 = null;

        if (vm.Attachment is { Length: > 0 })
        {
            if (vm.Attachment.Length > MAX_SIZE)
            {
                ModelState.AddModelError("Attachment", "The file exceeds the 5 MB limit.");
                return View("~/Views/Home/Index.cshtml", vm);
            }

            if (string.IsNullOrWhiteSpace(vm.Attachment.ContentType) || !vm.Attachment.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("Attachment", "Only image files are allowed.");
                return View("~/Views/Home/Index.cshtml", vm);
            }

            await using var ms = new MemoryStream();
            await vm.Attachment.CopyToAsync(ms, cancellationToken);
            base64 = Convert.ToBase64String(ms.ToArray());
        }
        
        var message = await _messageRepository.CreateModel(vm.Name ?? "", vm.Message, DateTimeOffset.UtcNow);
        
        _logger.LogInformation("Message saved in the DataBase | Message: {id}, {Message}", message.Id, message.Message);

        var text = "⚠️ NOTIFICATION ⚠️️\n\n" +
                   $"👤 {message.Username}\n" +
                   "---------------------------------\n\n" +
                   $"💬\n\n{message.Message}\n" +
                   "---------------------------------";

        try
        {
            IEnumerable<string>? attachments = base64 is not null ? new[] { base64 } : null;
            await _signal.SendAsync(_signalSettings.SendMessageTo, text, attachments, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Signal send failed");
            ModelState.AddModelError("", "Failed to send via Signal. Please try again later.");
            return View("~/Views/Home/Index.cshtml", vm);
        }
        
        TempData["MessageSent"] = true;
        
        return RedirectToAction("Index", "Home");
    }
}