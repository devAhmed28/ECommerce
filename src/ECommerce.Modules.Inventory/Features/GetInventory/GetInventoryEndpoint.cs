using ECommerce.Modules.Inventory.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace ECommerce.Modules.Inventory.Features.GetInventory;

[ApiController]
[Route("api/inventory")]
[Authorize]
public sealed class GetInventoryEndpoint(IInventoryService inventoryService) : ControllerBase
{
    [HttpGet("{productId:int}")]
    public async Task<IActionResult> GetInventory(int productId, CancellationToken cancellationToken)
    {
        var inventory = await inventoryService.GetAsync(productId, cancellationToken);

        if (inventory is null)
        {
            return NotFound(new
            {
                error = "Inventory record was not found."
            });
        }

        return Ok(new GetInventoryResponse
        {
            Inventory = inventory
        });
    }
}