namespace ECommerce.Modules.Catalog.Features.CreateProduct;

public sealed class CreateProductCommand
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
}