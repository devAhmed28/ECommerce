namespace ECommerce.Modules.Catalog.Features.Products.SearchProducts;

public sealed class SearchProductsQuery
{
    public string? Name { get; set; }
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? Status { get; set; }

    public string SortBy { get; set; } = "Id";
    public string SortOrder { get; set; } = "asc";

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}