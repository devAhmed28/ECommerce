using CartEntity = ECommerce.Modules.Cart.Domain.Entities.Cart;

using ECommerce.Modules.Cart.Application.Interfaces;
using ECommerce.Modules.Cart.Infrastructure.Database;
using Dapper;
using ECommerce.Modules.Cart.Domain.Entities;

namespace ECommerce.Modules.Cart.Infrastructure.Repositories;

public sealed class CartRepository : ICartRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

	public CartRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<CartEntity?> GetByUserIdAsync(Guid userId)
    {
        const string sql = @"
            SELECT
                Id,
                UserId,
                CreatedAt,
                UpdatedAt
            FROM Carts
            WHERE UserId = @UserId;
        ";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<CartEntity>(sql, new 
        { 
            UserId =  userId 
        });
    }

    public async Task<CartEntity?> GetByIdAsync(Guid cartId)
    {
        const string sql = @"
            SELECT
                Id,
                UserId,
                CreatedAt,
                UpdatedAt
            FROM Carts
            WHERE Id = @CartId;
        ";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<CartEntity>(sql, new
        {
            CartId = cartId
        });
    }

    public async Task AddAsync(CartEntity cart)
    {
        const string sql = @"
            INSERT INTO Carts
            (
                Id,
                UserId,
                CreatedAt,
                UpdatedAt
            )
            VALUES
            (
                @Id,
                @UserId,
                @CreatedAt,
                @UpdatedAt
            );
        ";

        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(sql, cart);
    }

    public async Task AddItemAsync(CartItem item)
    {
        const string sql = @"
            INSERT INTO CartItems
            (
                Id,
                CartId,
                ProductId,
                Quantity
            )
            VALUES
            (
                @Id,
                @CartId,
                @ProductId,
                @Quantity
            );
        ";

        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(sql, item);
    }

    public async Task UpdateItemQuantityAsync(Guid cartItemId, int quantity)
    {
        const string sql = @"
            UPDATE CartItems
            SET Quantity = @Quantity
            WHERE Id = @CartItemId;
        ";

        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(sql, new
        {
            CartItemId = cartItemId,
            Quantity = quantity
        });
    }

    public async Task RemoveItemAsync(Guid cartItemId)
    {
        const string sql = @"
            DELETE FROM CartItems
            WHERE Id = @CartItemId;
        ";

        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(sql, new
        {
            CartItemId = cartItemId
        });
    }

    public async Task ClearItemsAsync(Guid cartId)
    {
        const string sql = @"
            DELETE FROM CartItems
            WHERE CartId = @CartId;
        ";

        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(sql, new
        {
            CartId = cartId
        });
    }

    public async Task<IReadOnlyList<CartItem>> GetItemsAsync(Guid cartId)
    {
        const string sql = @"
            SELECT
                Id,
                CartId,
                ProductId,
                Quantity
            FROM CartItems
            WHERE CartId = @CartId
            ORDER BY Id;
        ";

        using var connection = _connectionFactory.CreateConnection();

        var items = await connection.QueryAsync<CartItem>(sql, new
        {
            CartId = cartId
        });

        return items.AsList();
    }

    public async Task<CartItem?> GetItemAsync(Guid cartId, int productId)
    {
        const string sql = @"
            SELECT
                Id,
                CartId,
                ProductId,
                Quantity
            FROM CartItems
            WHERE CartId = @CartId
              AND ProductId = @ProductId;
        ";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<CartItem>(sql, new
        {
            CartId = cartId,
            ProductId = productId
        });
    }

    public async Task<CartItem?> GetItemByIdAsync(Guid cartId, Guid cartItemId)
    {
        const string sql = @"
            SELECT
                Id,
                CartId,
                ProductId,
                Quantity
            FROM CartItems
            WHERE CartId = @CartId
              AND Id = @CartItemId;
        ";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<CartItem>(sql, new
        {
            CartId = cartId,
            CartItemId = cartItemId
        });
    }

    public async Task UpdateCartTimestampAsync(Guid cartId)
    {
        const string sql = @"
            UPDATE Carts
            SET UpdatedAt = @UpdatedAt
            WHERE Id = @CartId;
        ";

        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(sql, new
        {
            CartId = cartId,
            UpdatedAt = DateTime.UtcNow
        });
    }
}