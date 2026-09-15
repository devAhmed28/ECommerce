using Dapper;
using ECommerce.Modules.Catalog.Infrastructure.Database;

namespace ECommerce.Modules.Catalog.Features.Categories.DeleteCategory;

public sealed class DeleteCategoryHandler
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DeleteCategoryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> HandleAsync(int id)
    {
        const string sql = @"
            DELETE FROM Categories
            WHERE Id = @Id;
            ";

        using var connection = _connectionFactory.CreateConnection();

        var affectedRows = await connection.ExecuteAsync(sql, 
                new 
                {
                    Id = id 
                });

        return affectedRows > 0;
    }
}