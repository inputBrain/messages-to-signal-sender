using System.ComponentModel.DataAnnotations;

namespace SmsSenderClient.Models;

public class MessageViewModel
{
    [Required(ErrorMessage = "Обов'язкове поле")]
    [MinLength(2, ErrorMessage = "Ім'я не може бути меншим ніж 2 символи.")]
    [MaxLength(50, ErrorMessage = "Ім'я не може перевищувати 50 символів.")]
    public string? Name { get; set; }
    
    [MaxLength(500, ErrorMessage = "Повідомлення не може перевищувати 500 символів.")]
    [Required(ErrorMessage = "Обов'язкове поле")]
    public string Message { get; set; }
    
    
    [Display(Name = "Вкладення")]
    public IFormFile? Attachment { get; set; }
}