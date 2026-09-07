using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ECommerce.Modules.Identity.Features.ForgotPassword;

[ApiController]
[Route("api/identity/forgot-password")]
[EnableRateLimiting("auth")]
public sealed class ForgotPasswordEndpoint : ControllerBase
{
    private readonly ForgotPasswordHandler _handler;
    private readonly IValidator<ForgotPasswordCommand> _validator;

    public ForgotPasswordEndpoint(ForgotPasswordHandler handler, IValidator<ForgotPasswordCommand> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        var validationResult = await _validator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                errors = validationResult.Errors.Select(x => x.ErrorMessage)
            });
        }

        await _handler.HandleAsync(command);

        return Ok(new
        {
            message = "If an account exists for this email, a password reset link has been sent."
        });
    }
}