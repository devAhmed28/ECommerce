namespace ECommerce.Modules.Cart.Domain.Entities;
public sealed class Cart
{
	private Cart()
	{
	}

	public Cart(Guid userId)
	{
		Id = Guid.NewGuid();
		UserId = userId;
		CreatedAt = DateTime.UtcNow;
		UpdatedAt = DateTime.UtcNow;
	}

	public Guid Id { get; private set; }
	public Guid UserId { get; private set; }
	public DateTime CreatedAt { get; private set; }
	public DateTime UpdatedAt { get; private set; }
}