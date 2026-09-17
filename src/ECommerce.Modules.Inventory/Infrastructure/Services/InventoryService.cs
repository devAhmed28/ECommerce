using Dapper;
using ECommerce.Modules.Inventory.Application.DTOs;
using ECommerce.Modules.Inventory.Application.Interfaces;
using ECommerce.Modules.Inventory.Infrastructure.Database;

namespace ECommerce.Modules.Inventory.Infrastructure.Services;

public sealed class InventoryService(IDbConnectionFactory connectionFactory) : IInventoryService
{
    public async Task<InventoryDto?> GetAsync(int productId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = @"
            SELECT
                ProductId,
                Quantity,
                ReservedQuantity,
                Quantity - ReservedQuantity AS AvailableQuantity,
                CreatedAt,
                UpdatedAt
            FROM InventoryItems
            WHERE ProductId = @ProductId;
            ";

        return await connection.QuerySingleOrDefaultAsync<InventoryDto>(
            new CommandDefinition(sql, 
                new 
                {
                    ProductId = productId 
                },
                cancellationToken: cancellationToken));
    }

    public async Task AdjustAsync(int productId, int quantity, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            const string existingSql = @"
                SELECT ReservedQuantity
                FROM InventoryItems WITH (UPDLOCK, ROWLOCK)
                WHERE ProductId = @ProductId;
                ";

            var reservedQuantity =
                await connection.QuerySingleOrDefaultAsync<int?>(
                    new CommandDefinition(existingSql, 
                        new 
                        { 
                            ProductId = productId 
                        },
                        transaction,cancellationToken: cancellationToken));

            if (reservedQuantity.HasValue)
            {
                if (quantity < reservedQuantity.Value)
                    throw new InvalidOperationException("Quantity cannot be less than the reserved quantity.");

                const string updateSql = @"
                    UPDATE InventoryItems
                    SET Quantity = @Quantity,
                        UpdatedAt = GETUTCDATE()
                    WHERE ProductId = @ProductId;
                    ";

                await connection.ExecuteAsync(
                    new CommandDefinition(updateSql, 
                        new
                        {
                            ProductId = productId,
                            Quantity = quantity
                        },
                        transaction, cancellationToken: cancellationToken));
            }
            else
            {
                const string insertSql = @"
                    INSERT INTO InventoryItems
                    (
                        ProductId,
                        Quantity,
                        ReservedQuantity,
                        CreatedAt,
                        UpdatedAt
                    )
                    VALUES
                    (
                        @ProductId,
                        @Quantity,
                        0,
                        GETUTCDATE(),
                        GETUTCDATE()
                    );
                    ";

                await connection.ExecuteAsync(
                    new CommandDefinition(insertSql,
                        new
                        {
                            ProductId = productId,
                            Quantity = quantity
                        },
                        transaction, cancellationToken: cancellationToken));
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task ReserveAsync(int productId, Guid orderId, int quantity, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            const string stockSql = @"
                SELECT Quantity, ReservedQuantity
                FROM InventoryItems WITH (UPDLOCK, ROWLOCK)
                WHERE ProductId = @ProductId;
                ";

            var stock =
                await connection.QuerySingleOrDefaultAsync<(int Quantity, int ReservedQuantity)>(
                    new CommandDefinition(
                        stockSql,
                        new { ProductId = productId },
                        transaction,
                        cancellationToken: cancellationToken));

            if (stock == default)
                throw new InvalidOperationException("Inventory record was not found.");

            var availableQuantity = stock.Quantity - stock.ReservedQuantity;

            if (availableQuantity < quantity)
                throw new InvalidOperationException("Insufficient available stock.");

            const string updateSql = @"
                UPDATE InventoryItems
                SET ReservedQuantity = ReservedQuantity + @Quantity,
                    UpdatedAt = GETUTCDATE()
                WHERE ProductId = @ProductId;
                ";

            await connection.ExecuteAsync(
                new CommandDefinition(
                    updateSql,
                    new
                    {
                        ProductId = productId,
                        Quantity = quantity
                    },
                    transaction, cancellationToken: cancellationToken));

            const string reservationSql = @"
                INSERT INTO StockReservations
                (
                    Id,
                    ProductId,
                    OrderId,
                    Quantity,
                    CreatedAt
                )
                VALUES
                (
                    @Id,
                    @ProductId,
                    @OrderId,
                    @Quantity,
                    GETUTCDATE()
                );
                ";

            await connection.ExecuteAsync(
                new CommandDefinition(
                    reservationSql,
                    new
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        OrderId = orderId,
                        Quantity = quantity
                    },
                    transaction, cancellationToken: cancellationToken));

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task ReleaseAsync(int productId, Guid orderId, int quantity, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            const string reservationSql = @"
                SELECT TOP (1)
                    Id,
                    Quantity
                FROM StockReservations WITH (UPDLOCK, ROWLOCK)
                WHERE ProductId = @ProductId
                  AND OrderId = @OrderId
                  AND ReleasedAt IS NULL
                ORDER BY CreatedAt;
                ";

            var reservation =
                await connection.QuerySingleOrDefaultAsync<
                    (Guid Id, int Quantity)>(
                    new CommandDefinition(
                        reservationSql,
                        new
                        {
                            ProductId = productId,
                            OrderId = orderId
                        },
                        transaction, cancellationToken: cancellationToken));

            if (reservation == default)
                throw new InvalidOperationException("Active stock reservation was not found.");

            if (reservation.Quantity != quantity)
                throw new InvalidOperationException("Release quantity does not match the reservation.");

            const string updateInventorySql = @"
                UPDATE InventoryItems
                SET ReservedQuantity = ReservedQuantity - @Quantity,
                    UpdatedAt = GETUTCDATE()
                WHERE ProductId = @ProductId
                  AND ReservedQuantity >= @Quantity;
                ";

            var updatedRows =
                await connection.ExecuteAsync(
                    new CommandDefinition(
                        updateInventorySql,
                        new
                        {
                            ProductId = productId,
                            Quantity = quantity
                        },
                        transaction, cancellationToken: cancellationToken));

            if (updatedRows == 0)
                throw new InvalidOperationException("Unable to release reserved stock.");

            const string releaseReservationSql = @"
                UPDATE StockReservations
                SET ReleasedAt = GETUTCDATE()
                WHERE Id = @Id;
                ";

            await connection.ExecuteAsync(
                new CommandDefinition(
                    releaseReservationSql,
                    new 
                    {
                        reservation.Id 
                    },
                    transaction, cancellationToken: cancellationToken));

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}