using ECommerce.Modules.Identity.Features.Login;

namespace ECommerce.Modules.Identity.Features.RefreshToken;

public sealed record RefreshTokenResult(LoginResponse? Response, bool IsReuseDetected);