using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ECommerce.Modules.Identity.Features.ResetPassword;

[ApiController]
[Route("api/identity/reset-password")]
[EnableRateLimiting("auth")]
public sealed class ResetPasswordEndpoint : ControllerBase
{
    private readonly ResetPasswordHandler _handler;
    private readonly IValidator<ResetPasswordCommand> _validator;

    public ResetPasswordEndpoint(ResetPasswordHandler handler, IValidator<ResetPasswordCommand> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var validationResult = await _validator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                errors = validationResult.Errors
                    .Select(x => x.ErrorMessage)
            });
        }

        var succeeded = await _handler.HandleAsync(command);

        if (!succeeded)
        {
            return BadRequest(new
            {
                error = "Unable to reset password."
            });
        }

        return Ok(new
        {
            message = "Password has been reset successfully."
        });
    }
}