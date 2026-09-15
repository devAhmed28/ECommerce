using Dapper;
using ECommerce.Modules.Catalog.Infrastructure.Database;

namespace ECommerce.Modules.Catalog.Features.Categories.CreateCategory;

public sealed class CreateCategoryHandler
{
    private readonly IDbConnectionFactory _connectionFactory;

	public CreateCategoryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<CreateCategoryResponse> HandleAsync(CreateCategoryCommand command)
    {
        var createdAt = DateTime.UtcNow;

        const string sql = @"
            INSERT INTO Categories
                (Name, Description, CreatedAt)
            VALUES
                (@Name, @Description, @CreatedAt);

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

        using var connection = _connectionFactory.CreateConnection();

        var categoryId =
            await connection.ExecuteScalarAsync<int>(sql, 
                new
                {
                    command.Name,
                    command.Description,
                    CreatedAt = createdAt
                });

        return new CreateCategoryResponse
        {
            Id = categoryId,
            Name = command.Name,
            Description = command.Description,
            CreatedAt = createdAt
        };
    }
}