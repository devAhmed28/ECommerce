namespace ECommerce.Modules.Identity.Infrastructure.Authentication;

public interface IRefreshTokenHasher
{
    string Hash(string refreshToken);
}