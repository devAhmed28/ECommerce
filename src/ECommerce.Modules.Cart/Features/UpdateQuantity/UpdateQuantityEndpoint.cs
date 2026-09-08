using ECommerce.Modules.Cart.Application.Interfaces;
using ECommerce.Shared.Abstractions;
using FluentValidation;
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
    private readonly IValidator<UpdateQuantityRequest> _validator;

    public UpdateQuantityEndpoint(ICartRepository cartRepository, ICurrentUser currentUser, IValidator<UpdateQuantityRequest> validator)
    {
        _cartRepository = cartRepository;
        _currentUser = currentUser;
        _validator = validator;
    }

    [HttpPut]
    public async Task<IActionResult> UpdateQuantity(UpdateQuantityRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

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

        await _cartRepository.UpdateCartTimestampAsync(cart.Id);

        return NoContent();
    }
}