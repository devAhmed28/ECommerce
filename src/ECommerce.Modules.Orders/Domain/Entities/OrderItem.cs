namespace ECommerce.Modules.Orders.Domain.Entities;
public sealed class OrderItem
{
    private OrderItem()
    {
    }

    public OrderItem(Guid orderId, int productId, string productName, decimal unitPrice, int quantity)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public int ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }

    public decimal LineTotal => UnitPrice * Quantity;
}