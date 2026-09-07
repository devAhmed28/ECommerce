namespace ECommerce.Modules.Identity.Features.Login;
public sealed record LoginResponse(string AccessToken, DateTime ExpiresAt, string refreshToken);
