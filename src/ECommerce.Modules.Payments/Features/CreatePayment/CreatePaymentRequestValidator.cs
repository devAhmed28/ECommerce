using FluentValidation;

namespace ECommerce.Modules.Payments.Features.CreatePayment;

public sealed class CreatePaymentRequestValidator
    : AbstractValidator<CreatePaymentRequest>
{
    public CreatePaymentRequestValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required.");
    }
}