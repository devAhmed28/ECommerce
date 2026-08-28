using Dapper;
using ECommerce.Modules.Catalog.Domain.Entities;
using ECommerce.Modules.Catalog.Infrastructure.Database;

namespace ECommerce.Modules.Catalog.Features.CreateProduct;

public sealed class CreateProductHandler
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CreateProductHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<CreateProductResponse> HandleAsync(CreateProductCommand command)
    {
        var product = new Product
        {
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            CategoryId = command.CategoryId,
            Status = ProductStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        const string sql = @"
            INSERT INTO Products (Name, Description, Price, CategoryId, Status, CreatedAt)
            VALUES (@Name, @Description, @Price, @CategoryId, @Status, @CreatedAt);
            
            SELECT CAST(SCOPE_IDENTITY() AS INT);
        ";

        using var connection = _connectionFactory.CreateConnection();

        var productId = await connection.ExecuteScalarAsync<int>(
            sql,
            new
            {
                product.Name,
                product.Description,
                product.Price,
                product.CategoryId,
                product.Status,
                product.CreatedAt
            }
        );

        return new CreateProductResponse
        {
            Id = productId,
            Name = product.Name,
            Price = product.Price,
            CategoryId = product.CategoryId,
            CreatedAt = product.CreatedAt
        };
    }
}