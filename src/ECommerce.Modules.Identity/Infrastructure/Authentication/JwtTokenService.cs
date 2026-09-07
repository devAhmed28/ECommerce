using ECommerce.Modules.Identity.Domain.Entities;
using ECommerce.Modules.Identity.Features.Login;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ECommerce.Modules.Identity.Infrastructure.Authentication;
public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _jwtOptions;

    public JwtTokenService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public AccessTokenResult CreateAccessToken(ApplicationUser user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),

            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(issuer: _jwtOptions.Issuer, audience: _jwtOptions.Audience, claims:  claims, expires: expiresAt, signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessTokenResult(accessToken, expiresAt);
    }

    public string CreateRefreshToken()
    {
        var randomBytes = new byte[64];

        RandomNumberGenerator.Fill(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }
}
