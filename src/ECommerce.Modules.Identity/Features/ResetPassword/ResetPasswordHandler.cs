using ECommerce.Modules.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Modules.Identity.Features.ResetPassword;

public sealed class ResetPasswordHandler
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ResetPasswordHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> HandleAsync(ResetPasswordCommand command)
    {
        var user = await _userManager.FindByEmailAsync(
            command.Email);

        if (user is null)
        {
            return false;
        }

        var result = await _userManager.ResetPasswordAsync(user, command.Token, command.NewPassword);

        return result.Succeeded;
    }
}