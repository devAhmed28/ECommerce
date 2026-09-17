namespace ECommerce.Modules.Inventory.Features.AdjustStock;

public sealed class AdjustStockRequest
{
    public int ProductId { get; init; }

    public int Quantity { get; init; }
}