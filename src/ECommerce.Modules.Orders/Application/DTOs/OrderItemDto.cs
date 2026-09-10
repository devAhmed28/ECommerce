namespace ECommerce.Modules.Orders.Application.DTOs;
public sealed class OrderItemDto
{
    public Guid Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
    public decimal LineTotal { get; init; }
}