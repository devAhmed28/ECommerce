namespace ECommerce.Modules.Inventory.Domain.Entities;

public sealed class StockReservation
{
    private StockReservation()
    {
    }

    public StockReservation(int productId, Guid orderId, int quantity)
    {
        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId));

        if (orderId == Guid.Empty)
            throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));

        Id = Guid.NewGuid();
        ProductId = productId;
        OrderId = orderId;
        Quantity = quantity;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public int ProductId { get; private set; }
    public Guid OrderId { get; private set; }
    public int Quantity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReleasedAt { get; private set; }

    public void Release()
    {
        if (ReleasedAt.HasValue)
            return;

        ReleasedAt = DateTime.UtcNow;
    }
}