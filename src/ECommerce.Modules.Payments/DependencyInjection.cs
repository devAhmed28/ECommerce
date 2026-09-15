using ECommerce.Modules.Payments.Application.Interfaces;
using ECommerce.Modules.Payments.Application.Services;
using ECommerce.Modules.Payments.Features.CreatePayment;
using ECommerce.Modules.Payments.Infrastructure.Database;
using ECommerce.Modules.Payments.Infrastructure.Repositories;
using ECommerce.Modules.Payments.Infrastructure.Stripe;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Modules.Payments;

public static class DependencyInjection
{
    public static IServiceCollection AddPaymentsModule(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionString)
    {
        var stripeSection = configuration.GetSection(StripeOptions.SectionName);

        var stripeOptions = stripeSection.Get<StripeOptions>()
            ?? throw new InvalidOperationException("Stripe configuration was not found.");

        if (string.IsNullOrWhiteSpace(stripeOptions.SecretKey))
            throw new InvalidOperationException("Stripe:SecretKey is not configured.");

        if (string.IsNullOrWhiteSpace(stripeOptions.PublishableKey))
            throw new InvalidOperationException("Stripe:PublishableKey is not configured.");

        if (string.IsNullOrWhiteSpace(stripeOptions.WebhookSecret))
            throw new InvalidOperationException("Stripe:WebhookSecret is not configured.");

        if (string.IsNullOrWhiteSpace(stripeOptions.Currency))
            throw new InvalidOperationException("Stripe:Currency is not configured.");

        services.Configure<StripeOptions>(stripeSection);

        services.AddScoped<IDbConnectionFactory>(_ => new SqlConnectionFactory(connectionString));

        services.AddScoped<IPaymentRepository, PaymentRepository>();

        services.AddScoped<IStripePaymentGateway, StripePaymentGateway>();

        services.AddScoped<PaymentService>();

        services.AddValidatorsFromAssemblyContaining<CreatePaymentRequestValidator>();

        return services;
    }
}