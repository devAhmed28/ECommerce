namespace ECommerce.Modules.Cart.Features.AddItem;
public sealed class AddItemRequest
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
}