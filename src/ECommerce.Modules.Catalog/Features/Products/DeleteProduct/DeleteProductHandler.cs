using Dapper;
using ECommerce.Modules.Catalog.Infrastructure.Database;

namespace ECommerce.Modules.Catalog.Features.Products.DeleteProduct;
public sealed class DeleteProductHandler
{
    private readonly IDbConnectionFactory _connectionFactory;

	public DeleteProductHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> HandleAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
            DELETE FROM Products
            WHERE Id = @Id;
        ";

        var affectedRows = await connection.ExecuteAsync(sql, new
        {
            Id = id
        });

        return affectedRows > 0;
    }
}
