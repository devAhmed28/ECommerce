namespace ECommerce.Modules.Payments.Application.DTOs;

public sealed record CreatePaymentDto(
    PaymentDto Payment,
    string ClientSecret,
    string PublishableKey);
