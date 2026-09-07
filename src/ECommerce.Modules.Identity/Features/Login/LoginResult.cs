namespace ECommerce.Modules.Identity.Features.Login;
public sealed record LoginResult(bool Succeeded, string? Error, LoginResponse? Response)
{
	public static LoginResult Success(LoginResponse response) => new(true, null, response);
	public static LoginResult Failure(string error) => new(false, error, null);
}