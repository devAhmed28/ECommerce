using System.Data;

namespace ECommerce.Modules.Cart.Infrastructure.Database;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}