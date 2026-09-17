using FluentValidation;

namespace ECommerce.Modules.Inventory.Features.ReserveStock;

public sealed class ReserveStockValidator : AbstractValidator<ReserveStockRequest>
{
    public ReserveStockValidator()
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