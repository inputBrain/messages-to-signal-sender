using Microsoft.Extensions.Logging;
using SmsSenderClient.Database.Message;

namespace SmsSenderClient.Database;

public class DatabaseFacade : IDatabaseFacade
{
    public IMessageRepository MessageRepository { get; set; }

    public DatabaseFacade(PostgreSqlContext context, ILoggerFactory loggerFactory)
    {
        MessageRepository = new MessageRepository(context, loggerFactory);
    }

}