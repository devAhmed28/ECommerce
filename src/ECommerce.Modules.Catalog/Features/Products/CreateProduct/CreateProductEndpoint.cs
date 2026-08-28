using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Catalog.Features.CreateProduct;

[ApiController]
[Route("api/catalog/products")]
public sealed class CreateProductEndpoint : ControllerBase
{
    private readonly CreateProductHandler _handler;
    private readonly IValidator<CreateProductCommand> _validator;

    public CreateProductEndpoint(CreateProductHandler handler, IValidator<CreateProductCommand> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
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