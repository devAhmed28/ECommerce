namespace ECommerce.Modules.Cart.Application.DTOs;
public sealed class CartItemDto
{
    public Guid Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}