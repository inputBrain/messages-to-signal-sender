using Microsoft.AspNetCore.Mvc;
using SmsSenderClient.Database.Message;
using SmsSenderClient.Models;

namespace SmsSenderClient.Controllers;

public class MessageController : Controller
{
    private readonly ILogger<MessageController> _logger;
    private readonly IMessageRepository _messageRepository;


    public MessageController(ILogger<MessageController> logger, IMessageRepository messageRepository)
    {
        _logger = logger;
        _messageRepository = messageRepository;
    }


    [HttpPost]
    public async Task<IActionResult> SendMessage(MessageViewModel vm)
    {
        _logger.LogInformation("Sending message");
        _logger.LogInformation("Name: {VmName} | Message: {VmMessage}", vm.Name, vm.Message);
        
        return RedirectToAction("Index", "Home");
    }
}