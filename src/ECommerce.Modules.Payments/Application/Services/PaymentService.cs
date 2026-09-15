using ECommerce.Modules.Orders.Application.Contracts;
using ECommerce.Modules.Payments.Application.DTOs;
using ECommerce.Modules.Payments.Application.Interfaces;
using ECommerce.Modules.Payments.Domain.Entities;
using ECommerce.Modules.Payments.Infrastructure.Stripe;
using Microsoft.Extensions.Options;

namespace ECommerce.Modules.Payments.Application.Services;

public sealed class PaymentService(
    IPaymentRepository paymentRepository,
    IStripePaymentGateway stripePaymentGateway,
    IOrderPaymentReader orderPaymentReader,
    IOptions<StripeOptions> stripeOptions)
{
    public async Task<CreatePaymentDto> CreateAsync(Guid userId, Guid orderId, string idempotencyKey, CancellationToken cancellationToken)
    {
        var options = stripeOptions.Value;

        var existingByKey = await paymentRepository.GetByIdempotencyKeyAsync(userId, idempotencyKey);

        if (existingByKey is not null)
        {
            if (existingByKey.OrderId != orderId)
                throw new InvalidOperationException("The idempotency key has already been used for another order.");

            if (existingByKey.Status == PaymentStatus.Succeeded)
                throw new InvalidOperationException("This order has already been paid.");

            if (existingByKey.Status == PaymentStatus.Refunded)
                throw new InvalidOperationException("This order's payment has already been refunded.");

            return await ResumeExistingPaymentAsync(existingByKey, userId, cancellationToken);
        }

        var existingByOrder = await paymentRepository.GetByOrderIdAsync(orderId, userId);

        if (existingByOrder is not null)
        {
            if (existingByOrder.Status == PaymentStatus.Succeeded)
                throw new InvalidOperationException("This order has already been paid.");

            if (existingByOrder.Status == PaymentStatus.Refunded)
                throw new InvalidOperationException("This order's payment has already been refunded.");

            return await ResumeExistingPaymentAsync(existingByOrder, userId, cancellationToken);
        }

        var order = await orderPaymentReader.GetPayableOrderAsync(orderId, userId, cancellationToken);

        if (order is null)
            throw new InvalidOperationException("The order does not exist, does not belong to the current user, or is not payable.");

        var payment = new Payment(order.OrderId, order.UserId, order.TotalAmount, options.Currency, idempotencyKey);

        await paymentRepository.CreatePendingAsync(payment);

        var intent =
            await stripePaymentGateway.CreatePaymentIntentAsync(
                payment.Id,
                payment.OrderId,
                payment.Amount,
                payment.Currency,
                payment.IdempotencyKey,
                cancellationToken);

        await paymentRepository.AttachStripePaymentIntentAsync(payment.Id, intent.PaymentIntentId);

        var updated = await paymentRepository.GetByIdAsync(payment.Id, userId)
            ?? throw new InvalidOperationException("The payment could not be loaded after creation.");

        return new CreatePaymentDto(ToDto(updated), intent.ClientSecret, options.PublishableKey);
    }

    private async Task<CreatePaymentDto> ResumeExistingPaymentAsync(Payment payment, Guid userId, CancellationToken cancellationToken)
    {
        var options = stripeOptions.Value;

        if (!string.IsNullOrWhiteSpace(payment.StripePaymentIntentId))
        {
            var intent = await stripePaymentGateway.GetPaymentIntentAsync(payment.StripePaymentIntentId, cancellationToken);

            return new CreatePaymentDto(ToDto(payment), intent.ClientSecret, options.PublishableKey);
        }

        var newIntent = 
            await stripePaymentGateway.CreatePaymentIntentAsync(
                payment.Id,
                payment.OrderId,
                payment.Amount,
                payment.Currency,
                payment.IdempotencyKey,
                cancellationToken);

        await paymentRepository.AttachStripePaymentIntentAsync(payment.Id, newIntent.PaymentIntentId);

        var updated =
            await paymentRepository.GetByIdAsync(payment.Id, userId)
            ?? throw new InvalidOperationException("The payment could not be loaded after recovery.");

        return new CreatePaymentDto(ToDto(updated), newIntent.ClientSecret, options.PublishableKey);
    }

    public async Task<PaymentDto?> GetAsync(Guid paymentId, Guid userId)
    {
        var payment = await paymentRepository.GetByIdAsync(paymentId, userId);

        return payment is null ? null : ToDto(payment);
    }

    public async Task<PaymentDto> RefundAsync(
        Guid paymentId,
        Guid userId,
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        var payment = await paymentRepository.GetByIdAsync(paymentId, userId);

        if (payment is null)
            throw new KeyNotFoundException("Payment not found.");

        if (payment.Status == PaymentStatus.Refunded)
            return ToDto(payment);

        if (payment.Status != PaymentStatus.Succeeded)
            throw new InvalidOperationException("Only succeeded payments can be refunded.");

        if (string.IsNullOrWhiteSpace(payment.StripePaymentIntentId))
            throw new InvalidOperationException("The payment does not have a Stripe PaymentIntent.");

        var refund =
            await stripePaymentGateway.RefundAsync(
                payment.StripePaymentIntentId,
                payment.Id,
                idempotencyKey,
                cancellationToken);

        if (refund.Status == "succeeded")
        {
            await paymentRepository.MarkRefundedAsync(payment.Id, refund.RefundId);
        }

        var updated = 
            await paymentRepository.GetByIdAsync(payment.Id, userId)
            ?? throw new InvalidOperationException("The payment could not be loaded after refund.");

        return ToDto(updated);
    }

    public Task HandleSucceededWebhookAsync(string eventId, string eventType, string paymentIntentId)
    {
        return paymentRepository.ApplyWebhookAsync(eventId, eventType, paymentIntentId, PaymentStatus.Succeeded, null,
            null);
    }

    public Task HandleFailedWebhookAsync(string eventId, string eventType, string paymentIntentId, string? failureReason)
    {
        return paymentRepository.ApplyWebhookAsync(
            eventId,
            eventType,
            paymentIntentId,
            PaymentStatus.Failed,
            failureReason,
            null);
    }

    public Task HandleRefundedWebhookAsync(string eventId, string eventType, string paymentIntentId, string stripeRefundId)
    {
        return paymentRepository.ApplyWebhookAsync(
            eventId,
            eventType,
            paymentIntentId,
            PaymentStatus.Refunded,
            null,
            stripeRefundId);
    }

    private static PaymentDto ToDto(Payment payment)
    {
        return new PaymentDto(
            payment.Id,
            payment.OrderId,
            payment.Amount,
            payment.Currency,
            payment.Status,
            payment.CreatedAt,
            payment.UpdatedAt);
    }
}