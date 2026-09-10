using System.Data;

namespace ECommerce.Modules.Orders.Infrastructure.Database;
public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}