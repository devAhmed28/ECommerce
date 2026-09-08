using ECommerce.Modules.Cart.Application.DTOs;
using ECommerce.Modules.Cart.Application.Interfaces;
using ECommerce.Shared.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Cart.Features.GetCart;

[ApiController]
[Route("api/cart")]
[Authorize]
public sealed class GetCartEndpoint : ControllerBase
{
    private readonly ICartRepository _cartRepository;
    private readonly ICurrentUser _currentUser;

    public GetCartEndpoint(ICartRepository cartRepository, ICurrentUser currentUser)
    {
        _cartRepository = cartRepository;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var cart = await _cartRepository.GetByUserIdAsync(_currentUser.UserId);

        if (cart is null)
        {
            return NotFound();
        }

        var items = await _cartRepository.GetItemsAsync(cart.Id);

        var cartDto = new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            CreatedAt = cart.CreatedAt,
            UpdatedAt = cart.UpdatedAt,
            Items = items
                .Select(item => new CartItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }).ToList()
        };

        return Ok(cartDto);
    }
}