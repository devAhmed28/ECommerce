namespace ECommerce.Modules.Inventory.Domain.Entities;

public sealed class InventoryItem
{
    private InventoryItem()
    {
    }

    public InventoryItem(int productId, int quantity)
    {
        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId));

        if (quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        ProductId = productId;
        Quantity = quantity;
        ReservedQuantity = 0;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public int ProductId { get; private set; }

    public int Quantity { get; private set; }

    public int ReservedQuantity { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public int AvailableQuantity => Quantity - ReservedQuantity;

    

    public void AdjustQuantity(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        if (quantity < ReservedQuantity)
            throw new InvalidOperationException("Quantity cannot be less than the reserved quantity.");

        Quantity = quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reserve(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        if (AvailableQuantity < quantity)
            throw new InvalidOperationException("Insufficient available stock.");

        ReservedQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Release(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        if (ReservedQuantity < quantity)
            throw new InvalidOperationException("Cannot release more stock than currently reserved.");

        ReservedQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }
}
