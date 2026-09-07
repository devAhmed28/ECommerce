using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Catalog.Features.Products.SearchProducts;

[ApiController]
[Route("api/catalog/products")]
public sealed class SearchProductsEndpoint : ControllerBase
{
    private readonly SearchProductsHandler _handler;
    private readonly IValidator<SearchProductsQuery> _validator;

	public SearchProductsEndpoint(SearchProductsHandler searchProductsHandler, IValidator<SearchProductsQuery> validator)
    {
        _handler = searchProductsHandler;
        _validator = validator;
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts(
        [FromQuery] string? name,
        [FromQuery] int? categoryId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int? status,
        [FromQuery] string sortBy = "Id",
        [FromQuery] string sortOrder = "asc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new SearchProductsQuery
        {
            Name = name,
            CategoryId = categoryId,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            Status = status,
            SortBy = sortBy,
            SortOrder = sortOrder,
            Page = page,
            PageSize = pageSize
        };

        var validationResult = await _validator.ValidateAsync(query);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var result = await _handler.HandleAsync(query);

        return Ok(result);
    }
}