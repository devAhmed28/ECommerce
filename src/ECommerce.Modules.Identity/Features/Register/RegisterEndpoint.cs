using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Identity.Features.Register;

[ApiController]
[Route("api/identity/register")]
public sealed class RegisterEndpoint : ControllerBase
{
    private readonly RegisterHandler _handler;
    private readonly IValidator<RegisterCommand> _validator;

    public RegisterEndpoint(
        RegisterHandler handler,
        IValidator<RegisterCommand> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var validationResult = await _validator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var result = await _handler.HandleAsync(command);

        if (!result.Succeeded)
        {
            return Conflict(result.Errors);
        }

        return StatusCode(StatusCodes.Status201Created, result.Response);
    }
}