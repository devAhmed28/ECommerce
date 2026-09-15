using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Catalog.Features.Categories.GetCategories;

[ApiController]
[Route("api/catalog/categories")]
public sealed class GetCategoriesEndpoint : ControllerBase
{
    private readonly GetCategoriesHandler _handler;

    public GetCategoriesEndpoint(GetCategoriesHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _handler.HandlAsync();

        return Ok(categories);
    }
}
