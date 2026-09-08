using ECommerce.Modules.Cart.Application.DTOs;
using ECommerce.Modules.Cart.Application.Interfaces;
using ECommerce.Modules.Cart.Domain.Entities;
using ECommerce.Shared.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Cart.Features.AddItem;
[ApiController]
[Route("api/cart/items")]
[Authorize]
public sealed class AddItemEndpoint : ControllerBase
{
    private readonly ICartRepository _cartRepository;
    private readonly ICurrentUser _currentUser;

    public AddItemEndpoint(ICartRepository cartRepository, ICurrentUser currentUser)
    {
        _cartRepository = cartRepository;
        _currentUser = currentUser;
    }

    [HttpPost]
    public async Task<IActionResult> AddItem(AddItemRequest request)
    {
        var cart = await _cartRepository.GetByUserIdAsync(_currentUser.UserId);

        if (cart is null)
        {
            cart = new Domain.Entities.Cart(_currentUser.UserId);
            await _cartRepository.AddAsync(cart);
        }

        var existingItem = await _cartRepository.GetItemAsync(cart.Id, request.ProductId);

        if (existingItem is not null)
        {
            existingItem.IncreaseQuantity(request.Quantity);

            await _cartRepository.UpdateItemQuantityAsync(existingItem.Id, existingItem.Quantity);
        }
        else
        {
            var item = new CartItem(cart.Id, request.ProductId, request.Quantity);

            await _cartRepository.AddItemAsync(item);
        }

        await _cartRepository.UpdateCartTimestampAsync(cart.Id);

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