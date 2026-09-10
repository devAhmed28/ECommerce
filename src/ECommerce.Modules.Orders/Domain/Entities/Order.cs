namespace ECommerce.Modules.Orders.Domain.Entities;
public sealed class Order
{
    private Order()
    {
    }

    public Order(Guid userId, decimal totalAmount)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        TotalAmount = totalAmount;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public void Cancel()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("Only pending orders can be cancelled.");
        }

        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum OrderStatus
{
    Pending = 1,
    Confirmed = 2,
    Processing = 3,
    Shipped = 4,
    Delivered = 5,
    Cancelled = 6
}