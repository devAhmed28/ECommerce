namespace ECommerce.Shared.Abstractions.Inventory;

public interface IInventoryStockWriter
{
    Task ReserveAsync(Guid orderId, IReadOnlyCollection<InventoryReservationItem> items, CancellationToken cancellationToken = default);

    Task ReleaseAsync(Guid orderId, IReadOnlyCollection<InventoryReservationItem> items, CancellationToken cancellationToken = default);
}

public sealed record InventoryReservationItem(int ProductId, int Quantity);