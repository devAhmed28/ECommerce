using CartEntity = ECommerce.Modules.Cart.Domain.Entities.Cart;

using ECommerce.Modules.Cart.Domain.Entities;

namespace ECommerce.Modules.Cart.Application.Interfaces;

public interface ICartRepository
{
    Task<CartEntity?> GetByUserIdAsync(Guid userId);

    Task<CartEntity?> GetByIdAsync(Guid cartId);

    Task AddAsync(CartEntity cart);

    Task AddItemAsync(CartItem item);

    Task UpdateItemQuantityAsync(Guid cartItemId, int quantity);

    Task RemoveItemAsync(Guid cartItemId);

    Task ClearItemsAsync(Guid cartId);

    Task<IReadOnlyList<CartItem>> GetItemsAsync(Guid cartId);

    Task<CartItem?> GetItemAsync(Guid cartId, int productId);

    Task<CartItem?> GetItemByIdAsync(Guid cartId, Guid cartItemId);

    Task UpdateCartTimestampAsync(Guid cartId);
}