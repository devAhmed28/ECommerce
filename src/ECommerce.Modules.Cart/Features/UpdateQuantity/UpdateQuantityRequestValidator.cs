using FluentValidation;

namespace ECommerce.Modules.Cart.Features.UpdateQuantity;
public sealed class UpdateQuantityRequestValidator : AbstractValidator<UpdateQuantityRequest>
{
	public UpdateQuantityRequestValidator()
	{
		RuleFor(x => x.CartItemId)
			.NotEmpty();

		RuleFor(x => x.Quantity)
			.GreaterThan(0);
	}
}