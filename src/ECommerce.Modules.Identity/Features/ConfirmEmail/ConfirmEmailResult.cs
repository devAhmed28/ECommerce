namespace ECommerce.Modules.Identity.Features.ConfirmEmail;
public sealed record ConfirmEmailResult(bool Succeeded, string? Error)
{
	public static ConfirmEmailResult Success() => new(true, null);

	public static ConfirmEmailResult Failure(string error) => new(false, error);
}