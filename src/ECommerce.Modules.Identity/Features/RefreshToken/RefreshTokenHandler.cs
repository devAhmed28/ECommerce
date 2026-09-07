using ECommerce.Modules.Identity.Domain.Entities;
using ECommerce.Modules.Identity.Features.Login;
using ECommerce.Modules.Identity.Infrastructure.Authentication;
using ECommerce.Modules.Identity.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RefreshTokenEntity = ECommerce.Modules.Identity.Domain.Entities.RefreshToken;

namespace ECommerce.Modules.Identity.Features.RefreshToken;

public sealed class RefreshTokenHandler
{
    private readonly IdentityDbContext _identityDbContext;
    private readonly IRefreshTokenHasher _refreshTokenHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IClientIpAddressProvider _clientIpAddressProvider;

    public RefreshTokenHandler(IdentityDbContext dbContext, IRefreshTokenHasher refreshTokenHasher, IJwtTokenService jwtTokenService, UserManager<ApplicationUser> userManager, IClientIpAddressProvider clientIpAddressProvider)
    {
        _identityDbContext = dbContext;
        _refreshTokenHasher = refreshTokenHasher;
        _jwtTokenService = jwtTokenService;
        _userManager = userManager;
        _clientIpAddressProvider = clientIpAddressProvider;
    }

    public async Task<RefreshTokenResult> HandleAsync(RefreshTokenCommand command)
    {
        var tokenHash = _refreshTokenHasher.Hash(command.RefreshToken);

        var currentRefreshToken = await _identityDbContext.RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == tokenHash);

        if (currentRefreshToken is null)
        {
            return new RefreshTokenResult(null, false);
        }

        var now = DateTime.UtcNow;

        if (!currentRefreshToken.IsActive(now))
        {
            if (currentRefreshToken.WasReplaced)
            {
                return new RefreshTokenResult(null, true);
            }

            return new RefreshTokenResult(null, false);
        }

        var user = await _userManager.FindByIdAsync(currentRefreshToken.UserId.ToString());

        if (user is null)
        {
            return new RefreshTokenResult(null, false);
        }

        var accessToken = _jwtTokenService.CreateAccessToken(user);

        var newRawRefreshToken = _jwtTokenService.CreateRefreshToken();

        var newRefreshTokenHash = _refreshTokenHasher.Hash(newRawRefreshToken);

        var ipAddress = _clientIpAddressProvider.GetClientIpAddress();

        // create the replacement refresh-token entity.
        var newRefreshToken = new RefreshTokenEntity(user.Id, newRefreshTokenHash, now.AddDays(7), ipAddress, currentRefreshToken.FamilyId);

        // revoke the current token and link it to the replacement token.
        currentRefreshToken.Replace(newRefreshToken.Id, now, ipAddress);

        _identityDbContext.RefreshTokens.Add(newRefreshToken);

        try
        {
            await _identityDbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return new RefreshTokenResult(null, false);
        }

        return new RefreshTokenResult(new LoginResponse(accessToken.AccessToken, accessToken.ExpiresAt, newRawRefreshToken), false);
    }
}