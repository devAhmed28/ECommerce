using ECommerce.Modules.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Modules.Identity.Features.ConfirmEmail;

public sealed class ConfirmEmailHandler
{
    private readonly UserManager<ApplicationUser> _userManager;

	public ConfirmEmailHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ConfirmEmailResult> HandleAsync(ConfirmEmailCommand command)
    {
        var user = await _userManager.FindByIdAsync(command.UserId.ToString());

        if (user == null)
        {
            return ConfirmEmailResult.Failure("Invalid email confirmation request.");
        }

        if (user.EmailConfirmed)
        {
            return ConfirmEmailResult.Success();
        }

        var result = await _userManager.ConfirmEmailAsync(user, command.Token);

        if (!result.Succeeded)
        {
            return ConfirmEmailResult.Failure(
                "Invalid or expired email confirmation token.");
        }

        return ConfirmEmailResult.Success();
    }
}