using System.Data;

namespace ECommerce.Modules.Inventory.Infrastructure.Database;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}