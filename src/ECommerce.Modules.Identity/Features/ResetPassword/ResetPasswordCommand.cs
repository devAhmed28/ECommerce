namespace ECommerce.Modules.Identity.Features.ResetPassword;

public sealed record ResetPasswordCommand(string Email, string Token, string NewPassword);