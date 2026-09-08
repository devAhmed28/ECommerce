using ECommerce.Modules.Cart.Application.Interfaces;
using ECommerce.Shared.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Cart.Features.RemoveItem;
[ApiController]
[Route("api/cart/items")]
[Authorize]
public sealed class RemoveItemEndpoint : ControllerBase
{
    private readonly ICartRepository _cartRepository;
    private readonly ICurrentUser _currentUser;

    public RemoveItemEndpoint(ICartRepository cartRepository, ICurrentUser currentUser)
    {
        _cartRepository = cartRepository;
        _currentUser = currentUser;
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveItem(RemoveItemRequest request)
    {
        var cart = await _cartRepository.GetByUserIdAsync(_currentUser.UserId);

        if (cart is null)
        {
            return NotFound();
        }

        var item = await _cartRepository.GetItemByIdAsync(cart.Id, request.CartItemId);

        if (item is null)
        {
            return NotFound();
        }

        await _cartRepository.RemoveItemAsync(item.Id);

        await _cartRepository.UpdateCartTimestampAsync(cart.Id);

        return NoContent();
    }
}