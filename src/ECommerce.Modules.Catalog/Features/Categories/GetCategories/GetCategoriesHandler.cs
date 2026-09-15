using Dapper;
using ECommerce.Modules.Catalog.Infrastructure.Database;

namespace ECommerce.Modules.Catalog.Features.Categories.GetCategories;

public sealed class GetCategoriesHandler
{
    private readonly IDbConnectionFactory _connectionFactory;

	public GetCategoriesHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<GetCategoriesResponse>> HandlAsync()
    {
        const string sql = @"
            SELECT
                Id,
                Name,
                Description,
                CreatedAt,
                UpdatedAt
            FROM Categories
            ORDER BY Id;
            ";

        using var connection = _connectionFactory.CreateConnection();

        var categories = await connection.QueryAsync<GetCategoriesResponse>(sql);

        return categories.AsList();
    }
}