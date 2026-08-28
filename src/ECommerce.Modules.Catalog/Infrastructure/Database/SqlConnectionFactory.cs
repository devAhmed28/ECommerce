using Microsoft.Data.SqlClient;
using System.Data;

namespace ECommerce.Modules.Catalog.Infrastructure.Database;

public sealed class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

	public SqlConnectionFactory(string connectionString)
	{
		_connectionString = connectionString;
	}

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}