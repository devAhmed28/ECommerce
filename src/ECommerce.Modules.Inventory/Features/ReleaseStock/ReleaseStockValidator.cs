using FluentValidation;

namespace ECommerce.Modules.Inventory.Features.ReleaseStock;

public sealed class ReleaseStockValidator : AbstractValidator<ReleaseStockRequest>
{
    public ReleaseStockValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product ID must be greater than zero.");

        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");
    }
}