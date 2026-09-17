using System.Data;
using Microsoft.Data.SqlClient;

namespace ECommerce.Modules.Inventory.Infrastructure.Database;

public sealed class SqlConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        return new SqlConnection(connectionString);
    }
}