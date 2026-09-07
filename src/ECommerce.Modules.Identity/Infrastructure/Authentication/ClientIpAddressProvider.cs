using Microsoft.AspNetCore.Http;

namespace ECommerce.Modules.Identity.Infrastructure.Authentication;

public sealed class ClientIpAddressProvider : IClientIpAddressProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClientIpAddressProvider(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetClientIpAddress()
    {
        return _httpContextAccessor
            .HttpContext?
            .Connection
            .RemoteIpAddress?
            .ToString();
    }
}