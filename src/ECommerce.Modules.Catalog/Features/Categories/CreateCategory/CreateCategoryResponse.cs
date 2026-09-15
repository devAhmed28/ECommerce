namespace ECommerce.Modules.Catalog.Features.Categories.CreateCategory;

public sealed class CreateCategoryResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime CreatedAt { get; set; }
}