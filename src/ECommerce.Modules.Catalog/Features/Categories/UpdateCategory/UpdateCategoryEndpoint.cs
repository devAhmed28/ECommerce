using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Catalog.Features.Categories.UpdateCategory;

[ApiController]
[Route("api/catalog/categories")]
public sealed class UpdateCategoryEndpoint : ControllerBase
{
    private readonly UpdateCategoryHandler _handler;
    private readonly IValidator<UpdateCategoryCommand> _validator;

    public UpdateCategoryEndpoint(UpdateCategoryHandler handler, IValidator<UpdateCategoryCommand> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryCommand command)
    {
        var validationResult = await _validator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var result = await _handler.HandleAsync(id, command);

        if (result is null)
        {
            return NotFound(new
            {
                message = $"Category with ID {id} not found"
            });
        }

        return Ok(result);
    }
}
