using System.ComponentModel.DataAnnotations;

namespace SmsSenderClient.Models;

public class MessageViewModel
{
    public string? Name { get; set; }
    
    [MaxLength(500, ErrorMessage = "The message cannot exceed 500 characters.")]
    [Required(ErrorMessage = "The message field is required.")]
    public string Message { get; set; }
}