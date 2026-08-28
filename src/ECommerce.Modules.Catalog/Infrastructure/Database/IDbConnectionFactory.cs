using System.Data;

namespace ECommerce.Modules.Catalog.Infrastructure.Database;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}