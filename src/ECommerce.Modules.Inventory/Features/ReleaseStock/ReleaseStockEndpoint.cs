using ECommerce.Modules.Inventory.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Inventory.Features.ReleaseStock;

[ApiController]
[Route("api/inventory")]
[Authorize]
public sealed class ReleaseStockEndpoint(IInventoryService inventoryService, IValidator<ReleaseStockRequest> validator) : ControllerBase
{
    [HttpPost("{productId:int}/release")]
    public async Task<IActionResult> ReleaseStock(int productId, ReleaseStockRequest request, CancellationToken cancellationToken)
    {
        request = new ReleaseStockRequest
        {
            ProductId = productId,
            OrderId = request.OrderId,
            Quantity = request.Quantity
        };

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        try
        {
            await inventoryService.ReleaseAsync(productId, request.OrderId, request.Quantity, cancellationToken);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                error = ex.Message
            });
        }
    }
}