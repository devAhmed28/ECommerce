using ECommerce.Modules.Identity.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Modules.Identity.Features.ChangePassword;
[ApiController]
[Route("api/identity/change-password")]
[Authorize]
public sealed class ChangePasswordEndpoint : ControllerBase
{
    private readonly ChangePasswordHandler _handler;
    private readonly IValidator<ChangePasswordCommand> _validator;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChangePasswordEndpoint(ChangePasswordHandler handler, IValidator<ChangePasswordCommand> validator, UserManager<ApplicationUser> userManager)
    {
        _handler = handler;
        _validator = validator;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
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

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Unauthorized();
        }

        var succeeded = await _handler.HandleAsync(user, command);

        if (!succeeded)
        {
            return BadRequest(new
            {
                error = "Unable to change password."
            });
        }

        return Ok(new
        {
            message = "Password has been changed successfully."
        });
    }
}