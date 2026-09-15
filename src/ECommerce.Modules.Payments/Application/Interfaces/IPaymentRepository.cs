using ECommerce.Modules.Payments.Domain.Entities;

namespace ECommerce.Modules.Payments.Application.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid paymentId, Guid userId);

    Task<Payment?> GetByOrderIdAsync(Guid orderId, Guid userId);

    Task<Payment?> GetByIdempotencyKeyAsync(Guid userId, string idempotencyKey);

    Task CreatePendingAsync(Payment payment); 

    Task AttachStripePaymentIntentAsync(Guid paymentId, string stripePaymentIntentId);

    Task MarkRefundedAsync(Guid paymentId, string stripeRefundId);

    Task<bool> ApplyWebhookAsync(
        string eventId,
        string eventType,
        string stripePaymentIntentId,
        PaymentStatus status,
        string? failureReason,
        string? stripeRefundId);
}