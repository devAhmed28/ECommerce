using Dapper;
using ECommerce.Modules.Catalog.Infrastructure.Database;

namespace ECommerce.Modules.Catalog.Features.Categories.GetCategory;

public sealed class GetCategoryHandler
{
    private readonly IDbConnectionFactory _connectionFactory;

	public GetCategoryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<GetCategoryResponse?> HandleAsync(int id)
    {
        const string sql = @"
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

        return await connection.QuerySingleOrDefaultAsync<GetCategoryResponse>(sql,
            new
            {
                Id = id
            });
    }
}
