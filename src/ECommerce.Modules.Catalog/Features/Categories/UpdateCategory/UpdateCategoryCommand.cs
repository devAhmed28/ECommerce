namespace ECommerce.Modules.Catalog.Features.Categories.UpdateCategory;

public sealed class UpdateCategoryCommand
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}