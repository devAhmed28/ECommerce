using ECommerce.Modules.Payments.Application.Interfaces;
using Microsoft.Extensions.Options;
using Stripe;

namespace ECommerce.Modules.Payments.Infrastructure.Stripe;

public sealed class StripePaymentGateway : IStripePaymentGateway
{
    private readonly PaymentIntentService _paymentIntentService;
    private readonly RefundService _refundService;

    public StripePaymentGateway(IOptions<StripeOptions> options)
    {
        var client = new StripeClient(options.Value.SecretKey);

        _paymentIntentService = new PaymentIntentService(client);
        _refundService = new RefundService(client);
    }

    public async Task<StripePaymentIntentResult> CreatePaymentIntentAsync(
        Guid paymentId,
        Guid orderId,
        decimal amount,
        string currency,
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        var amountInMinorUnits = checked((long)Math.Round(amount * 100m, MidpointRounding.AwayFromZero));

        var options = new PaymentIntentCreateOptions
        {
            Amount = amountInMinorUnits,
            Currency = currency,
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true
            },
            Metadata = new Dictionary<string, string>
            {
                ["payment_id"] = paymentId.ToString(),
                ["order_id"] = orderId.ToString()
            }
        };

        var requestOptions = new RequestOptions
        {
            IdempotencyKey = $"payment:{idempotencyKey}"
        };

        var paymentIntent = await _paymentIntentService.CreateAsync(options, requestOptions, cancellationToken);

        if (string.IsNullOrWhiteSpace(paymentIntent.ClientSecret))
            throw new InvalidOperationException("Stripe did not return a PaymentIntent client secret.");

        return new StripePaymentIntentResult(paymentIntent.Id, paymentIntent.ClientSecret);
    }

    public async Task<StripePaymentIntentResult> GetPaymentIntentAsync(string paymentIntentId, CancellationToken cancellationToken)
    {
        var paymentIntent = await _paymentIntentService.GetAsync(paymentIntentId, cancellationToken: cancellationToken);

        if (string.IsNullOrWhiteSpace(paymentIntent.ClientSecret))
            throw new InvalidOperationException("Stripe did not return a PaymentIntent client secret.");

        return new StripePaymentIntentResult(paymentIntent.Id, paymentIntent.ClientSecret);
    }

    public async Task<StripeRefundResult> RefundAsync(
        string paymentIntentId,
        Guid paymentId,
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        var options = new RefundCreateOptions
        {
            PaymentIntent = paymentIntentId,
            Metadata = new Dictionary<string, string>
            {
                ["payment_id"] = paymentId.ToString()
            }
        };

        var requestOptions = new RequestOptions
        {
            IdempotencyKey = $"refund:{paymentId}:{idempotencyKey}"
        };

        var refund = await _refundService.CreateAsync(options, requestOptions, cancellationToken);

        return new StripeRefundResult(refund.Id, refund.Status ?? "unknown");
    }
}