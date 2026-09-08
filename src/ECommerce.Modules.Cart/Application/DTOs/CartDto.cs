namespace ECommerce.Modules.Cart.Application.DTOs;
public sealed class CartDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public IReadOnlyList<CartItemDto> Items { get; init; } = [];
}