using FluentValidation;

namespace ECommerce.Modules.Catalog.Features.Products.SearchProducts;
public sealed class SearchProductsValidator : AbstractValidator<SearchProductsQuery>
{
	public SearchProductsValidator()
	{
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinPrice.HasValue);

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxPrice.HasValue);

        RuleFor(x => x)
            .Must(x =>
                !x.MinPrice.HasValue ||
                !x.MaxPrice.HasValue ||
                x.MinPrice <= x.MaxPrice)
            .WithMessage(
                "Minimum price cannot be greater than maximum price.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .When(x => x.CategoryId.HasValue);

        RuleFor(x => x.Status)
            .InclusiveBetween(1, 3)
            .When(x => x.Status.HasValue);

        RuleFor(x => x.SortOrder)
            .Must(x =>
                x.Equals("asc", StringComparison.OrdinalIgnoreCase) ||
                x.Equals("desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Sort order must be 'asc' or 'desc'.");
    }
}
