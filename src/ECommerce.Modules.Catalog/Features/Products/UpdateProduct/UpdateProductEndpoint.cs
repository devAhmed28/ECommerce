using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Catalog.Features.Products.UpdateProduct;
[ApiController]
[Route("api/catalog/products")]
public sealed class UpdateProductEndpoint : ControllerBase
{
    private readonly UpdateProductHandler _handler;
    private readonly IValidator<UpdateProductCommand> _validator;

    public UpdateProductEndpoint(UpdateProductHandler handler, IValidator<UpdateProductCommand> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductCommand command)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "Product id must be greater than 0."
            });
        }

        var validationResult = await _validator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var result = await _handler.HandleAsync(id, command);

            if (result is null)
            {
                return NotFound(new
                {
                    message = $"Product with id {id} was not found."
                });
            }

            return Ok(result);
        }
        catch(InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message,
            });
        }
    }
}