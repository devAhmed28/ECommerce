using ECommerce.Modules.Cart.Application.Interfaces;
using ECommerce.Shared.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Cart.Features.UpdateQuantity;
[ApiController]
[Route("api/cart/items")]
[Authorize]
public sealed class UpdateQuantityEndpoint : ControllerBase
{
    private readonly ICartRepository _cartRepository;
    private readonly ICurrentUser _currentUser;

    public UpdateQuantityEndpoint(ICartRepository cartRepository, ICurrentUser currentUser)
    {
        _cartRepository = cartRepository;
        _currentUser = currentUser;
    }

    [HttpPut]
    public async Task<IActionResult> UpdateQuantity(UpdateQuantityRequest request)
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

        await _cartRepository.UpdateItemQuantityAsync(item.Id, request.Quantity);

        return NoContent();
    }
}