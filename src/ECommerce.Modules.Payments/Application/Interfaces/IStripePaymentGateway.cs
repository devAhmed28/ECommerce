namespace ECommerce.Modules.Payments.Application.Interfaces;

public interface IStripePaymentGateway
{
    Task<StripePaymentIntentResult> CreatePaymentIntentAsync(
        Guid paymentId,
        Guid orderId,
        decimal amount,
        string currency,
        string idempotencyKey,
        CancellationToken cancellationToken);

    Task<StripePaymentIntentResult> GetPaymentIntentAsync(
        string paymentIntentId,
        CancellationToken cancellationToken);

    Task<StripeRefundResult> RefundAsync(
        string paymentIntentId,
        Guid paymentId,
        string idempotencyKey,
        CancellationToken cancellationToken);
}

public sealed record StripePaymentIntentResult(string PaymentIntentId, string ClientSecret);

public sealed record StripeRefundResult(string RefundId, string Status);