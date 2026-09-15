using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Catalog.Features.Categories.CreateCategory;

[ApiController]
[Route("api/catalog/categories")]
public sealed class CreateCategoryEndpoint : ControllerBase
{
    private readonly CreateCategoryHandler _handler;
    private readonly IValidator<CreateCategoryCommand> _validator;

    public CreateCategoryEndpoint(CreateCategoryHandler createCategoryHandler, IValidator<CreateCategoryCommand> validator)
    {
        _handler = createCategoryHandler;
        _validator = validator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody]  CreateCategoryCommand command)
    {
        var validationResult = await _validator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var result = await _handler.HandleAsync(command);

        return StatusCode(StatusCodes.Status201Created, result);
    }
}