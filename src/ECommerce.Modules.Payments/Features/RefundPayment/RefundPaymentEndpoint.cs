using ECommerce.Modules.Payments.Application.Services;
using ECommerce.Shared.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Payments.Features.RefundPayment;

[ApiController]
[Route("api/payments")]
[Authorize]
public sealed class RefundPaymentEndpoint(PaymentService paymentService, ICurrentUser currentUser) : ControllerBase
{
    [HttpPut("{paymentId:guid}/refund")]
    public async Task<IActionResult> RefundPayment(Guid paymentId, CancellationToken cancellationToken)
    {
        var idempotencyKey = Request.Headers["Idempotency-Key"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            return BadRequest(new
            {
                error = "The Idempotency-Key header is required."
            });
        }

        try
        {
            var payment = await paymentService.RefundAsync(paymentId, currentUser.UserId, idempotencyKey, cancellationToken);

            return Ok(payment);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                error = ex.Message
            });
        }
    }
}