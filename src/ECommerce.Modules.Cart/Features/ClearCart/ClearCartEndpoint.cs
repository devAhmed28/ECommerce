using ECommerce.Modules.Cart.Application.Interfaces;
using ECommerce.Shared.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Cart.Features.ClearCart;
[ApiController]
[Route("api/cart")]
[Authorize]
public sealed class ClearCartEndpoint : ControllerBase
{
    private readonly ICartRepository _cartRepository;
    private readonly ICurrentUser _currentUser;

    public ClearCartEndpoint(ICartRepository cartRepository, ICurrentUser currentUser)
    {
        _cartRepository = cartRepository;
        _currentUser = currentUser;
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var cart = await _cartRepository.GetByUserIdAsync(_currentUser.UserId);

        if (cart is null)
        {
            return NotFound();
        }

        await _cartRepository.ClearItemsAsync(cart.Id);

        return NoContent();
    }
}