using ECommerce.Modules.Identity.Domain.Entities;
using ECommerce.Modules.Identity.Infrastructure.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ECommerce.Modules.Identity.Features.Register;

public sealed class RegisterHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly EmailOptions _emailOptions;

    public RegisterHandler(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IOptions<EmailOptions> emailOptions)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _emailOptions = emailOptions.Value;
    }

    public async Task<RegisterResult> HandleAsync(RegisterCommand command)
    {
        var user = new ApplicationUser
        {
            UserName = command.Email,
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName
        };

        var result = await _userManager.CreateAsync(
            user,
            command.Password);

        if (!result.Succeeded)
        {
            return RegisterResult.Failure(result.Errors);
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var encodedToken = Uri.EscapeDataString(token);

        var confirmationUrl = $"{_emailOptions.BaseUrl}/api/identity/confirm-email?userId={user.Id}&token={encodedToken}";

        await _emailSender.SendAsync(user.Email!, "Verify your email address", $@"<h1>Welcome to E-Commerce!</h1>
        <p>Please verify your email address by clicking the link below:</p>
        <p>
            <a href=""{confirmationUrl}"">
                Verify Email
            </a>
        </p>
        <p>This verification link was generated for your account.</p>
        <p>If you did not create this account, you can ignore this email.</p>");

        return RegisterResult.Success(
            new RegisterResponse
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailConfirmed = user.EmailConfirmed
            });
    }
}