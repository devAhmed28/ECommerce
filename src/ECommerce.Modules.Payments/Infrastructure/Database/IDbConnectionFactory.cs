using System.Data;

namespace ECommerce.Modules.Payments.Infrastructure.Database;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}