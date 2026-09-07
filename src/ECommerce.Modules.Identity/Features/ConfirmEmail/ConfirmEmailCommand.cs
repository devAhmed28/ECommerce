namespace ECommerce.Modules.Identity.Features.ConfirmEmail;
public sealed record ConfirmEmailCommand(Guid UserId, string Token);
