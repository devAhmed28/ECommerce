namespace ECommerce.Modules.Inventory.Application.DTOs;

public sealed class InventoryDto
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
    public int ReservedQuantity { get; init; }
    public int AvailableQuantity { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}