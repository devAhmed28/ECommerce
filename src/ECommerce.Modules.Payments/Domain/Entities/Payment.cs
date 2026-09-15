namespace ECommerce.Modules.Payments.Domain.Entities;

public sealed class Payment
{
    private Payment()
    {
    }

    public Payment(
        Guid orderId,
        Guid userId,
        decimal amount,
        string currency,
        string idempotencyKey)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException("Order ID is required.", nameof(orderId));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID is required.", nameof(userId));
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Payment amount must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency is required.", nameof(currency));
        }

        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            throw new ArgumentException("Idempotency key is required.", nameof(idempotencyKey));
        }

        Id = Guid.NewGuid();
        OrderId = orderId;
        UserId = userId;
        Amount = amount;
        Currency = currency.ToLowerInvariant();
        IdempotencyKey = idempotencyKey;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid OrderId { get; private set; }

    public Guid UserId { get; private set; }

    public decimal Amount { get; private set; }

    public string Currency { get; private set; } = string.Empty;

    public PaymentStatus Status { get; private set; }

    public string IdempotencyKey { get; private set; } = string.Empty;

    public string? StripePaymentIntentId { get; private set; }

    public string? StripeRefundId { get; private set; }

    public string? FailureReason { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public void SetStripePaymentIntent(string stripePaymentIntentId)
    {
        if (string.IsNullOrWhiteSpace(stripePaymentIntentId))
        {
            throw new ArgumentException("Stripe PaymentIntent ID is required.", nameof(stripePaymentIntentId));
        }

        StripePaymentIntentId = stripePaymentIntentId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkSucceeded()
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new InvalidOperationException("Only pending payments can be marked as succeeded.");
        }

        Status = PaymentStatus.Succeeded;
        FailureReason = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkFailed(string? failureReason)
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new InvalidOperationException("Only pending payments can be marked as failed.");
        }

        Status = PaymentStatus.Failed;
        FailureReason = failureReason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkRefunded(string stripeRefundId)
    {
        if (Status != PaymentStatus.Succeeded)
        {
            throw new InvalidOperationException("Only succeeded payments can be refunded.");
        }

        if (string.IsNullOrWhiteSpace(stripeRefundId))
        {
            throw new ArgumentException("Stripe refund ID is required.", nameof(stripeRefundId));
        }

        Status = PaymentStatus.Refunded;
        StripeRefundId = stripeRefundId;
        UpdatedAt = DateTime.UtcNow;
    }
}