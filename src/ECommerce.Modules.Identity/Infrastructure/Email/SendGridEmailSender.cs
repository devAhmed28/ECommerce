
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ECommerce.Modules.Identity.Infrastructure.Email;
public sealed class SendGridEmailSender : IEmailSender
{
    private readonly SendGridClient _sendGridClient;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public SendGridEmailSender(IConfiguration configuration)
    {
        var apiKey = configuration["SendGrid:ApiKey"] ??
            throw new InvalidOperationException("SendGrid API key is not configured.");

        _fromEmail = configuration["SendGrid:FromEmail"]
            ?? throw new InvalidOperationException(
                "SendGrid sender email is not configured.");

        _fromName = configuration["SendGrid:FromName"]
            ?? "E-Commerce App";

        _sendGridClient = new SendGridClient(apiKey);
    }

    public async Task SendAsync(string recipient, string subject, string body)
    {
        var from = new EmailAddress(_fromEmail, _fromName);
        var to = new EmailAddress(recipient);

        var message = MailHelper.CreateSingleEmail(from, to, subject, body, body);

        var response = await _sendGridClient.SendEmailAsync(message);

        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Body.ReadAsStringAsync();

            throw new InvalidOperationException($"SendGrid failed to send email. Status: {response.StatusCode}. Response: {responseBody}");
        }
    }
}
