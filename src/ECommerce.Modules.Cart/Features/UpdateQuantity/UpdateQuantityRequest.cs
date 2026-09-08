namespace ECommerce.Modules.Cart.Features.UpdateQuantity;
public sealed class UpdateQuantityRequest
{
    public Guid CartItemId { get; init; }
    public int Quantity { get; init; }
}