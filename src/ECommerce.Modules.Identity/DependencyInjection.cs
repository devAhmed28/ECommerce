using ECommerce.Modules.Identity.Domain.Entities;
using ECommerce.Modules.Identity.Features.ChangePassword;
using ECommerce.Modules.Identity.Features.ConfirmEmail;
using ECommerce.Modules.Identity.Features.ForgotPassword;
using ECommerce.Modules.Identity.Features.Login;
using ECommerce.Modules.Identity.Features.Logout;
using ECommerce.Modules.Identity.Features.RefreshToken;
using ECommerce.Modules.Identity.Features.Register;
using ECommerce.Modules.Identity.Features.ResetPassword;
using ECommerce.Modules.Identity.Infrastructure.Authentication;
using ECommerce.Modules.Identity.Infrastructure.Email;
using ECommerce.Modules.Identity.Infrastructure.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ECommerce.Modules.Identity;
public static class DependencyInjection
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration, string connectionString)
    {
        services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddDataProtection();
        services.AddHttpContextAccessor();

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.RequireUniqueEmail = true;

            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;

            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.AllowedForNewUsers = true;
        })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        services.AddScoped<RegisterHandler>();
        services.AddScoped<ConfirmEmailHandler>();
        services.AddScoped<LoginHandler>();
        services.AddScoped<RefreshTokenHandler>();
        services.AddScoped<LogoutHandler>();
        services.AddScoped<ForgotPasswordHandler>();
        services.AddScoped<ResetPasswordHandler>();
        services.AddScoped<ChangePasswordHandler>();

        services.AddValidatorsFromAssemblyContaining<RegisterValidator>();

        services.Configure<EmailOptions>(configuration.GetSection("Email"));
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));


        var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>() ?? throw new InvalidOperationException("JWT configuration is missing.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),

                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,

                    ValidateLifetime = true,

                    ClockSkew = TimeSpan.Zero
                };
            });


        services.AddScoped<IEmailSender, SendGridEmailSender>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenHasher, RefreshTokenHasher>();
        services.AddScoped<IClientIpAddressProvider, ClientIpAddressProvider>();

        return services;
    }
}
