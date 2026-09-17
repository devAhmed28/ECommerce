using ECommerce.Modules.Inventory.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Inventory.Features.ReserveStock;

[ApiController]
[Route("api/inventory")]
[Authorize]
public sealed class ReserveStockEndpoint(IInventoryService inventoryService, IValidator<ReserveStockRequest> validator) : ControllerBase
{
    [HttpPost("{productId:int}/reserve")]
    public async Task<IActionResult> ReserveStock(int productId, ReserveStockRequest request, CancellationToken cancellationToken)
    {
        request = new ReserveStockRequest
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
            await inventoryService.ReserveAsync(productId, request.OrderId, request.Quantity, cancellationToken);

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