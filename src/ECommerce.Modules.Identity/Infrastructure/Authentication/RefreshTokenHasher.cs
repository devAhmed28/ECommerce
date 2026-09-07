using System.Security.Cryptography;
using System.Text;

namespace ECommerce.Modules.Identity.Infrastructure.Authentication;

public sealed class RefreshTokenHasher : IRefreshTokenHasher
{
    public string Hash(string refreshToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));

        return Convert.ToHexString(bytes);
    }
}