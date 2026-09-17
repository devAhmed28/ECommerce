namespace ECommerce.Modules.Inventory.Features.ReserveStock;

public sealed class ReserveStockRequest
{
    public int ProductId { get; init; }

    public Guid OrderId { get; init; }

    public int Quantity { get; init; }
}