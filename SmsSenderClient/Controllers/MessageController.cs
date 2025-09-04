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
        
        var message = await _messageRepository.CreateModel(vm.Name ?? "", vm.Message, DateTimeOffset.UtcNow);
        
        _logger.LogInformation("Message saved in the DataBase | Message: {id}, {Message}", message.Id, message.Message);
        
        var text = $"-=WEBSITE MESSAGE=-\n\nName: {message.Username}\n\nMessage:\n{message.Message}";
        try
        {
            await _signal.SendTextAsync(_signalSettings.SendMessageTo, text, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Signal send failed");
        }
        
        TempData["MessageSent"] = true;
        
        return RedirectToAction("Index", "Home");
    }
}