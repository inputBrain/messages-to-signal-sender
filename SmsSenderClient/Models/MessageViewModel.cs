using System.ComponentModel.DataAnnotations;

namespace SmsSenderClient.Models;

public class MessageViewModel
{
    public string? Name { get; set; }
    
    [MaxLength(500)]
    [Required(ErrorMessage = " error")]
    public string Message { get; set; }
}