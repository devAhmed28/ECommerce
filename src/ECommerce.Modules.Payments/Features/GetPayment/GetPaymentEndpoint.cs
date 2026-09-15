using ECommerce.Modules.Payments.Application.Services;
using ECommerce.Shared.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Payments.Features.GetPayment;

[ApiController]
[Route("api/payments")]
[Authorize]
public sealed class GetPaymentEndpoint(PaymentService paymentService, ICurrentUser currentUser) : ControllerBase
{
    [HttpGet("{paymentId:guid}")]
    public async Task<IActionResult> GetPayment(Guid paymentId)
    {
        var payment = await paymentService.GetAsync(paymentId, currentUser.UserId); 

        return payment is null ? NotFound() : Ok(payment);
    }
}