using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmsSenderClient.Database.Message;

namespace SmsSenderClient.Database;

public class PostgreSqlContext : DbContext
{
    public readonly IDatabaseFacade Db;
    
    public DbSet<MessageModel> Message { get; set; }


    public PostgreSqlContext(DbContextOptions<PostgreSqlContext> options, ILoggerFactory loggerFactory) : base(options)
    {
        Db = new DatabaseFacade(this, loggerFactory);
    }
}