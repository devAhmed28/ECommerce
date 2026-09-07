using ECommerce.Modules.Identity.Domain.Entities;
using ECommerce.Modules.Identity.Infrastructure.Authentication;
using ECommerce.Modules.Identity.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Modules.Identity.Features.ChangePassword;
public sealed class ChangePasswordHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IdentityDbContext _identityDbContext;
    private readonly IClientIpAddressProvider _clientIpAddressProvider;


    public ChangePasswordHandler(UserManager<ApplicationUser> userManager, IdentityDbContext identityDbContext, IClientIpAddressProvider clientIpAddressProvider)
    {
        _userManager = userManager;
        _identityDbContext = identityDbContext;
        _clientIpAddressProvider = clientIpAddressProvider;
    }

    public async Task<bool> HandleAsync(ApplicationUser user, ChangePasswordCommand command)
    {
        var result = await _userManager.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword);

        if (!result.Succeeded)
        {
            return false;
        }

        var activeRefreshTokens = await _identityDbContext.RefreshTokens.Where(x => x.UserId == user.Id &&
        x.RevokedAt == null && x.ExpiresAt > DateTime.UtcNow).ToListAsync();

        var revokedAt = DateTime.UtcNow;

        foreach (var refreshToken in activeRefreshTokens)
        {
            refreshToken.Revoke(revokedAt, _clientIpAddressProvider.GetClientIpAddress());
        }

        await _identityDbContext.SaveChangesAsync();

        return true;
    }
}