using System;
using System.Threading.Tasks;

namespace SmsSenderClient.Database.Message;

public interface IMessageRepository
{
    Task<MessageModel> CreateModel(string username, string message, DateTimeOffset createdAt);
}