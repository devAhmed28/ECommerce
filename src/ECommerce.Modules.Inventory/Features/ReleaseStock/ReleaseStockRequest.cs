namespace ECommerce.Modules.Inventory.Features.ReleaseStock;

public sealed class ReleaseStockRequest
{
    public int ProductId { get; init; }

    public Guid OrderId { get; init; }

    public int Quantity { get; init; }
}