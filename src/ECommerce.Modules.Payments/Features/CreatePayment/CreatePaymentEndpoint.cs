using ECommerce.Modules.Payments.Application.Services;
using ECommerce.Shared.Abstractions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Payments.Features.CreatePayment;

[ApiController]
[Route("api/payments")]
[Authorize]
public sealed class CreatePaymentEndpoint(PaymentService paymentService, ICurrentUser currentUser, IValidator<CreatePaymentRequest> validator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreatePayment(CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

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
            var result = await paymentService.CreateAsync(currentUser.UserId, request.OrderId, idempotencyKey, cancellationToken);

            return Ok(result);
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