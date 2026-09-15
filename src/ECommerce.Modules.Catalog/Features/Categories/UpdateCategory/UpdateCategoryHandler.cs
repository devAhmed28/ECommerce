using Dapper;
using ECommerce.Modules.Catalog.Infrastructure.Database;

namespace ECommerce.Modules.Catalog.Features.Categories.UpdateCategory;

public sealed class UpdateCategoryHandler
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UpdateCategoryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<UpdateCategoryResponse?> HandleAsync(int id, UpdateCategoryCommand command)
    {
        var updatedAt = DateTime.UtcNow;

        const string sql = @"
            UPDATE Categories
            SET
                Name = @Name,
                Description = @Description,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id;

            SELECT
                Id,
                Name,
                Description,
                CreatedAt,
                UpdatedAt
            FROM Categories
            WHERE Id = @Id;
            ";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<UpdateCategoryResponse>(sql, 
            new
            {
                Id = id,
                command.Name,
                command.Description,
                UpdatedAt = updatedAt
            });
    }
}