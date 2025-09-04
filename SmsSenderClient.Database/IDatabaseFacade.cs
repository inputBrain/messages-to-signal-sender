using SmsSenderClient.Database.Message;

namespace SmsSenderClient.Database;

public interface IDatabaseFacade
{
    IMessageRepository MessageRepository { get; }
}