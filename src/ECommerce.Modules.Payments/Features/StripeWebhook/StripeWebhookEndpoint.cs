using ECommerce.Modules.Payments.Application.Services;
using ECommerce.Modules.Payments.Infrastructure.Stripe;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace ECommerce.Modules.Payments.Features.StripeWebhook;

[ApiController]
[Route("api/payments/webhook")]
public sealed class StripeWebhookEndpoint(PaymentService paymentService, IOptions<StripeOptions> stripeOptions) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Handle(CancellationToken cancellationToken)
    {
        var signature = Request.Headers["Stripe-Signature"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(signature))
            return BadRequest();

        if (string.IsNullOrWhiteSpace(stripeOptions.Value.WebhookSecret))
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
            new
            {
                error = "Stripe webhook secret is not configured."
            });
        }

        using var reader = new StreamReader(Request.Body);

        var json = await reader.ReadToEndAsync(cancellationToken);

        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                signature,
                stripeOptions.Value.WebhookSecret);
        }
        catch (StripeException)
        {
            return BadRequest();
        }

        switch (stripeEvent.Type)
        {
            case EventTypes.PaymentIntentSucceeded:
                if (stripeEvent.Data.Object is PaymentIntent succeededIntent)
                {
                    await paymentService.HandleSucceededWebhookAsync(
                        stripeEvent.Id,
                        stripeEvent.Type,
                        succeededIntent.Id);
                }

                break;

            case EventTypes.PaymentIntentPaymentFailed:
                if (stripeEvent.Data.Object is PaymentIntent failedIntent)
                {
                    await paymentService.HandleFailedWebhookAsync(
                        stripeEvent.Id,
                        stripeEvent.Type,
                        failedIntent.Id,
                        failedIntent.LastPaymentError?.Message);
                }

                break;

            case EventTypes.PaymentIntentCanceled:
                if (stripeEvent.Data.Object is PaymentIntent canceledIntent)
                {
                    await paymentService.HandleFailedWebhookAsync(
                        stripeEvent.Id,
                        stripeEvent.Type,
                        canceledIntent.Id,
                        "PaymentIntent was canceled.");
                }

                break;

            case EventTypes.RefundCreated:
            case EventTypes.RefundUpdated:
            case EventTypes.RefundFailed:
                if (stripeEvent.Data.Object is Refund refund &&
                    refund.PaymentIntentId is not null &&
                    refund.Status == "succeeded")
                {
                    await paymentService.HandleRefundedWebhookAsync(
                        stripeEvent.Id,
                        stripeEvent.Type,
                        refund.PaymentIntentId,
                        refund.Id);
                }

                break;
        }

        return Ok();
    }
}