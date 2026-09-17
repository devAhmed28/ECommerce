using ECommerce.Modules.Inventory.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Inventory.Features.AdjustStock;

[ApiController]
[Route("api/inventory")]
[Authorize(Roles = "Admin")]
public sealed class AdjustStockEndpoint(IInventoryService inventoryService, IValidator<AdjustStockRequest> validator) : ControllerBase
{
    [HttpPost("{productId:int}/adjust")]
    public async Task<IActionResult> AdjustStock(int productId, AdjustStockRequest request, CancellationToken cancellationToken)
    {
        request = new AdjustStockRequest
        {
            ProductId = productId,
            Quantity = request.Quantity
        };

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        try
        {
            await inventoryService.AdjustAsync(productId, request.Quantity, cancellationToken);

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