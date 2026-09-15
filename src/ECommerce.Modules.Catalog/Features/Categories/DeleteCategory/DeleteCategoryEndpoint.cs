using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Catalog.Features.Categories.DeleteCategory;

[ApiController]
[Route("api/catalog/categories")]
public sealed class DeleteCategoryEndpoint : ControllerBase
{
    private readonly DeleteCategoryHandler _handler;

    public DeleteCategoryEndpoint(DeleteCategoryHandler handler)
    {
        _handler = handler;
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var deleted = await _handler.HandleAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = $"Category with ID {id} not found"
            });
        }

        return Ok(new
        {
            message = $"Category with ID {id} deleted successfully"
        });
    }
}