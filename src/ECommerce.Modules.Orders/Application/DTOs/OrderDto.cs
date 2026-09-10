namespace ECommerce.Modules.Orders.Application.DTOs;
public sealed class OrderDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public decimal TotalAmount { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public IReadOnlyList<OrderItemDto> Items { get; init; } = [];
}