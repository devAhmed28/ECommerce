namespace ECommerce.Modules.Catalog.Features.Categories.UpdateCategory;

public sealed class UpdateCategoryResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}