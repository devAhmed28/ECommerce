namespace ECommerce.Modules.Catalog.Features.Products.UpdateProduct;
public sealed class UpdateProductCommand
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
}