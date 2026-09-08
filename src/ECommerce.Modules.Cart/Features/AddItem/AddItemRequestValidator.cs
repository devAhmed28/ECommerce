using FluentValidation;

namespace ECommerce.Modules.Cart.Features.AddItem;
public sealed class AddItemRequestValidator : AbstractValidator<AddItemRequest>
{
	public AddItemRequestValidator()
	{
		RuleFor(x => x.ProductId)
			.GreaterThan(0);

		RuleFor(x => x.Quantity)
			.GreaterThan(0);
	}
}