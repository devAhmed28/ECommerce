using Dapper;
using ECommerce.Modules.Catalog.Infrastructure.Database;

namespace ECommerce.Modules.Catalog.Features.GetProduct;

public sealed class GetProductHandler
{
    private readonly IDbConnectionFactory _connectionFactory;

	public GetProductHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<GetProductResponse?> HandleAsync(GetProductQuery query)
    {
        const string sql = @"
            SELECT
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.CategoryId,
                p.Status,
                p.CreatedAt,
                p.UpdatedAt,
                c.Name AS CategoryName
            FROM Products p
            INNER JOIN Categories c ON p.CategoryId = c.Id
            WHERE p.Id = @Id;
        ";

        using var connection = _connectionFactory.CreateConnection();

        var productDto = await connection.QueryFirstOrDefaultAsync(sql, new { query.Id });

        if (productDto is null)
        {
            return null;
        }

        return new GetProductResponse
        {
            Id = productDto.Id,
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            CategoryId = productDto.CategoryId,
            CategoryName = productDto.CategoryName,
            Status = productDto.Status.ToString(),
            CreatedAt = productDto.CreatedAt,
            UpdatedAt = productDto.UpdatedAt
        };
    }
}