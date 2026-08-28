using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Catalog.Features.GetProduct;

[ApiController]
[Route("api/catalog/products")]
public sealed class GetProductEndpoint : ControllerBase
{
    private readonly GetProductHandler _handler;

	public GetProductEndpoint(GetProductHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var query = new GetProductQuery
        {
            Id = id
        };

        var product = await _handler.HandleAsync(query);

        if (product is null)
        {
            return NotFound(new { Message = $"Product with ID {id} not found" });
        }

        return Ok(product);
    }
}