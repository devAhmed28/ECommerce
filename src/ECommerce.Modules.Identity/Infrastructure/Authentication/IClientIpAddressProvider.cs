namespace ECommerce.Modules.Identity.Infrastructure.Authentication;

public interface IClientIpAddressProvider
{
    string? GetClientIpAddress();
}