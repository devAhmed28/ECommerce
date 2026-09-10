namespace ECommerce.Modules.Orders.Domain.Entities;
public sealed class OrderStatusHistory
{
	private OrderStatusHistory()
	{
	}

	public OrderStatusHistory(Guid orderId, OrderStatus status)
	{
		Id = Guid.NewGuid();
		OrderId = orderId;
		Status = status;
		CreatedAt = DateTime.UtcNow;
	}

	public Guid Id { get; private set; }
	public Guid OrderId { get; private set; }
	public OrderStatus Status { get; private set; }
	public DateTime CreatedAt { get; private set; }
}