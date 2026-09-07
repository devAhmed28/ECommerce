namespace ECommerce.Modules.Identity.Infrastructure.Authentication;

public sealed record AccessTokenResult(string AccessToken, DateTime ExpiresAt);