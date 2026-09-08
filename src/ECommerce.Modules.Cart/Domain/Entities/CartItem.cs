namespace ECommerce.Modules.Cart.Domain.Entities;

public sealed class CartItem
{
    private CartItem()
    {
    }

    public CartItem(Guid cartId, int productId, int quantity)
    {
        Id = Guid.NewGuid();
        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
    }

    public Guid Id { get; private set; }
    public Guid CartId { get; private set; }
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }

    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
    }

    public void IncreaseQuantity(int  quantity)
    {
        Quantity += quantity;
    }
}