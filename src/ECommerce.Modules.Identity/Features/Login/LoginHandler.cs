using ECommerce.Modules.Identity.Domain.Entities;
using ECommerce.Modules.Identity.Infrastructure.Authentication;
using ECommerce.Modules.Identity.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using RefreshTokenEntity = ECommerce.Modules.Identity.Domain.Entities.RefreshToken;

namespace ECommerce.Modules.Identity.Features.Login;
public sealed class LoginHandler
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenHasher _refreshTokenHasher;
    private readonly IdentityDbContext _identityDbContext;
    private readonly IClientIpAddressProvider _clientIpAddressProvider;

    public LoginHandler(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService, IRefreshTokenHasher refreshTokenHasher, IdentityDbContext identityDbContext, IClientIpAddressProvider clientIpAddressProvider)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _refreshTokenHasher = refreshTokenHasher;
        _identityDbContext = identityDbContext;
        _clientIpAddressProvider = clientIpAddressProvider;
    }

    public async Task<LoginResult> HandleAsync(LoginCommand command)
    {
        var user = await _userManager.FindByEmailAsync(command.Email);

        if (user == null)
        {
            return LoginResult.Failure("Invalid email or password.");
        }

        if (!user.EmailConfirmed)
        {
            return LoginResult.Failure("Please confirm your email address before logging in.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, command.Password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            return LoginResult.Failure(
                "Invalid email or password.");
        }

        var accessToken = _jwtTokenService.CreateAccessToken(user);

        var refreshToken = _jwtTokenService.CreateRefreshToken();
        var refreshTokenHash = _refreshTokenHasher.Hash(refreshToken);

        var ipAddress = _clientIpAddressProvider.GetClientIpAddress();

        var familyId = Guid.NewGuid();

        var refreshTokenEntity = new RefreshTokenEntity(user.Id, refreshTokenHash, DateTime.UtcNow.AddDays(7), ipAddress, familyId);
        _identityDbContext.RefreshTokens.Add(refreshTokenEntity);
        await _identityDbContext.SaveChangesAsync();

        var response = new LoginResponse(accessToken.AccessToken, accessToken.ExpiresAt, refreshToken);

        return LoginResult.Success(response);
    }
}