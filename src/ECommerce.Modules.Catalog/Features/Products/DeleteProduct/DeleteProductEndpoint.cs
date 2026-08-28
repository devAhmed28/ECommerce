using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Catalog.Features.Products.DeleteProduct;
[ApiController]
[Route("api/catalog/products")]
public sealed class DeleteProductEndpoint : ControllerBase
{
    private readonly DeleteProductHandler _handler;

    public DeleteProductEndpoint(DeleteProductHandler handler)
    {
        _handler = handler;
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "Product id must be greater than 0."
            });
        }

        var deleted = await _handler.HandleAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = $"Product with id {id} was not found."
            });
        }

        return NoContent();
    }
}
