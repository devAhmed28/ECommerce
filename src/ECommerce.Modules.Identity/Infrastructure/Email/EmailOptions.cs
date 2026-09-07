namespace ECommerce.Modules.Identity.Infrastructure.Email;

public sealed class EmailOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string PasswordResetPath { get; set; } = "/reset-password";
}