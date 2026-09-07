using ECommerce.Modules.Identity.Infrastructure.Authentication;
using ECommerce.Modules.Identity.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Modules.Identity.Features.Logout;
public sealed class LogoutHandler
{
    private readonly IdentityDbContext _identityDbContext;
    private readonly IRefreshTokenHasher _tefreshTokenHasher;
    private readonly IClientIpAddressProvider _clientIpAddressProvider;

    public LogoutHandler(IdentityDbContext identityDbContext, IRefreshTokenHasher tefreshTokenHasher, IClientIpAddressProvider clientIpAddressProvider)
    {
        _identityDbContext = identityDbContext;
        _tefreshTokenHasher = tefreshTokenHasher;
        _clientIpAddressProvider = clientIpAddressProvider;
    }

    public async Task<bool> HandleAsync(LogoutCommand command)
    {
        var tokenHash = _tefreshTokenHasher.Hash(command.refreshToken);

        var refreshToken = await _identityDbContext.RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == tokenHash);

        if (refreshToken == null)
        {
            return false;
        }

        if (refreshToken.IsRevoked)
        {
            return false;
        }

        refreshToken.Revoke(DateTime.UtcNow, _clientIpAddressProvider.GetClientIpAddress());

        await _identityDbContext.SaveChangesAsync();

        return true;
    }
}
