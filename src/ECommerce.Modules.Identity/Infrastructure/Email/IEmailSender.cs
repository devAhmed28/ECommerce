namespace ECommerce.Modules.Identity.Infrastructure.Email;

public interface IEmailSender
{
    Task SendAsync(string recipient, string subject, string body);
}