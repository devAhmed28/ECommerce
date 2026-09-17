using ECommerce.Modules.Identity.Domain.Entities;

namespace ECommerce.Modules.Identity.Infrastructure.Authentication;
public interface IJwtTokenService
{
    Task<AccessTokenResult> CreateAccessToken(ApplicationUser user);
    string CreateRefreshToken();
}
