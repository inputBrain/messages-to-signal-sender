using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SmsSenderClient.Database.Message;

public class MessageRepository : AbstractRepository<MessageModel>, IMessageRepository
{
    public MessageRepository(PostgreSqlContext context, ILoggerFactory loggerFactory) : base(context, loggerFactory)
    {
    }


    public async Task<MessageModel> CreateModel(string username, string message, DateTimeOffset createdAt)
    {
        var model = MessageModel.CreateModel(username, message, createdAt);

        var result = await CreateModelAsync(model);
        if (result == null)
        {
            throw new Exception("Message model is not created");
        }

        return result;
    }
}