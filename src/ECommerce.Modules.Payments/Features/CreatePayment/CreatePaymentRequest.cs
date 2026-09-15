namespace ECommerce.Modules.Payments.Features.CreatePayment;

public sealed class CreatePaymentRequest
{
    public Guid OrderId { get; init; }
}