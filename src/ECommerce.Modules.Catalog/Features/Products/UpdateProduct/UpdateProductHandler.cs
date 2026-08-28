using Dapper;
using ECommerce.Modules.Catalog.Infrastructure.Database;

namespace ECommerce.Modules.Catalog.Features.Products.UpdateProduct;
public sealed class UpdateProductHandler
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UpdateProductHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<UpdateProductResponse?> HandleAsync(
        int id,
        UpdateProductCommand command)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string categoryExistsSql = @"
            SELECT COUNT(1)
            FROM Categories
            WHERE Id = @CategoryId;
            ";

        var categoryExists = await connection.ExecuteScalarAsync<int>(categoryExistsSql, new { command.CategoryId });

        if (categoryExists == 0 )
        {
            throw new InvalidOperationException($"Category with id {command.CategoryId} was not found.");
        }

        const string updateSql = @"
            UPDATE Products
            SET
                Name = @Name,
                Description = @Description,
                Price = @Price,
                CategoryId = @CategoryId,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id;
        ";

        var updatedAt = DateTime.UtcNow;

        var affectedRows = await connection.ExecuteAsync(updateSql, new
        {
            Id = id,
            command.Name,
            command.Description,
            command.Price,
            command.CategoryId,
            UpdatedAt = updatedAt
        });

        if (affectedRows == 0 )
        {
            return null;
        }

        const string selectSql = @"
            SELECT
                Id,
                Name,
                Description,
                Price,
                CategoryId,
                CreatedAt,
                UpdatedAt
            FROM Products
            WHERE Id = @Id;
        ";

        return await connection.QuerySingleAsync<UpdateProductResponse>(selectSql, new
        {
            Id = id,
        });
    }
}