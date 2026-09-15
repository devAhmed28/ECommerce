using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Catalog.Features.Categories.GetCategory;

[ApiController]
[Route("api/catalog/categories")]
public sealed class GetCategoryEndpoint : ControllerBase
{
    private readonly GetCategoryHandler _handler;

    public GetCategoryEndpoint(GetCategoryHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategory(int id)
    {
        var category = await _handler.HandleAsync(id);

        if (category is null)
        {
            return NotFound(new
            {
                message = $"Category with ID {id} not found"
            });
        }

        return Ok(category);
    }
}
