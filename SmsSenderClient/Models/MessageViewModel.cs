using System.ComponentModel.DataAnnotations;

namespace SmsSenderClient.Models;

public class MessageViewModel
{
    [MaxLength(50, ErrorMessage = "The name cannot exceed 50 characters.")]
    public string? Name { get; set; }
    
    [MaxLength(500, ErrorMessage = "The message cannot exceed 500 characters.")]
    [Required(ErrorMessage = "The message field is required.")]
    public string Message { get; set; }
}