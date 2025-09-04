using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmsSenderClient.Database.Message;

public class MessageModel : AbstractModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string Username { get; set; }
    
    public string Message { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }


    public static MessageModel CreateModel(string username, string message, DateTimeOffset createdAt)
    {
        return new MessageModel
        {
            Username = username,
            Message = message,
            CreatedAt = createdAt
        };
    }
}