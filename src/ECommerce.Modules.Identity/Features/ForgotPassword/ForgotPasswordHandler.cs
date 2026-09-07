using ECommerce.Modules.Identity.Domain.Entities;
using ECommerce.Modules.Identity.Infrastructure.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ECommerce.Modules.Identity.Features.ForgotPassword;
public sealed class ForgotPasswordHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly EmailOptions _emailOptions;

    public ForgotPasswordHandler(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IOptions<EmailOptions> emailOptions)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _emailOptions = emailOptions.Value;
    }

    public async Task HandleAsync(ForgotPasswordCommand command)
    {
        var user = await _userManager.FindByEmailAsync(command.Email);

        if (user == null)
        {
            return;
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        var resetUrl = $"{_emailOptions.BaseUrl}{_emailOptions.PasswordResetPath}?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(resetToken)}";

        var subject = "Reset your ECommerce password";

        var body = $@"Hello {user.FirstName},

            We received a request to reset your password.

            Click the link below to reset your password:

            {resetUrl}

            If you did not request this, you can safely ignore this email.

            This link is for password recovery only.
            ";

        await _emailSender.SendAsync(user.Email!, subject, body);
    }
}