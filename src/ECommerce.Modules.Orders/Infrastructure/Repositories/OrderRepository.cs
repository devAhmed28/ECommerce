using Dapper;
using ECommerce.Modules.Orders.Application.Interfaces;
using ECommerce.Modules.Orders.Domain.Entities;
using ECommerce.Modules.Orders.Infrastructure.Database;

namespace ECommerce.Modules.Orders.Infrastructure.Repositories;
public sealed class OrderRepository : IOrderRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public OrderRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task CreateAsync(Order order, IReadOnlyList<OrderItem> items, OrderStatusHistory statusHistory, Guid userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(
                @"
                INSERT INTO Orders
                (
                    Id,
                    UserId,
                    TotalAmount,
                    Status,
                    CreatedAt,
                    UpdatedAt
                )
                VALUES
                (
                    @Id,
                    @UserId,
                    @TotalAmount,
                    @Status,
                    @CreatedAt,
                    @UpdatedAt
                );
            ",
                order,
                transaction);

            await connection.ExecuteAsync(
                @"
                INSERT INTO OrderItems
                (
                    Id,
                    OrderId,
                    ProductId,
                    ProductName,
                    UnitPrice,
                    Quantity
                )
                VALUES
                (
                    @Id,
                    @OrderId,
                    @ProductId,
                    @ProductName,
                    @UnitPrice,
                    @Quantity
                );
            ",
                items,
                transaction);

            await connection.ExecuteAsync(
                @"
                INSERT INTO OrderStatusHistory
                (
                    Id,
                    OrderId,
                    Status,
                    CreatedAt
                )
                VALUES
                (
                    @Id,
                    @OrderId,
                    @Status,
                    @CreatedAt
                );
            ",
                statusHistory,
                transaction);

            await connection.ExecuteAsync(
                @"
                DELETE ci
                FROM CartItems ci
                INNER JOIN Carts c
                    ON c.Id = ci.CartId
                WHERE c.UserId = @UserId
            ",
                new
                {
                    UserId = userId
                }, transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<Order?> GetByIdAsync(Guid orderId, Guid userId)
    {
        const string sql = @"
            SELECT
                Id,
                UserId,
                TotalAmount,
                Status,
                CreatedAt,
                UpdatedAt
            FROM Orders
            WHERE Id = @OrderId
              AND UserId = @UserId;
        ";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<Order>(sql, new
        {
            OrderId = orderId,
            UserId = userId
        });
    }

    public async Task<IReadOnlyList<Order>> GetByUserIdAsync(Guid userId)
    {
        const string sql = @"
            SELECT
                Id,
                UserId,
                TotalAmount,
                Status,
                CreatedAt,
                UpdatedAt
            FROM Orders
            WHERE UserId = @UserId
            ORDER BY CreatedAt DESC;
        ";

        using var connection = _connectionFactory.CreateConnection();

        var orders = await connection.QueryAsync<Order>(sql, new
        {
            UserId = userId
        });

        return orders.AsList();
    }

    public async Task<IReadOnlyList<OrderItem>> GetItemsAsync(Guid orderId)
    {
        const string sql = @"
             SELECT
                 Id,
                 OrderId,
                 ProductId,
                 ProductName,
                 UnitPrice,
                 Quantity
             FROM OrderItems
             WHERE OrderId = @OrderId
             ORDER BY Id;
        ";

        using var connection = _connectionFactory.CreateConnection();

        var items = await connection.QueryAsync<OrderItem>(sql, new
        {
            OrderId = orderId
        });

        return items.AsList();
    }

    public async Task<OrderStatusHistory?> GetLatestStatusHistoryAsync(Guid orderId)
    {
        const string sql = @"
            SELECT TOP 1
                Id,
                OrderId,
                Status,
                CreatedAt
            FROM OrderStatusHistory
            WHERE OrderId = @OrderId
            ORDER BY CreatedAt DESC;
        ";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<OrderStatusHistory>(sql, new
        {
            OrderId = orderId
        });
    }

    public async Task UpdateStatusAsync(Guid orderId, Guid userId, OrderStatus status)
    {
        const string sql = @"
            UPDATE Orders
            SET
                Status = @Status,
                UpdatedAt = @UpdatedAt
            WHERE Id = @OrderId
              AND UserId = @UserId;
        ";

        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(sql, new
        {
            OrderId = orderId,
            UserId = userId,
            Status = status,
            UpdatedAt = DateTime.UtcNow
        });
    }

    public async Task AddStatusHistoryAsync(OrderStatusHistory statusHistory)
    {
        const string sql = @"
            INSERT INTO OrderStatusHistory
            (
                Id,
                OrderId,
                Status,
                CreatedAt
            )
            VALUES
            (
                @Id,
                @OrderId,
                @Status,
                @CreatedAt
            );
        ";

        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(sql, statusHistory);
    }
}