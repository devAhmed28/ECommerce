namespace ECommerce.Modules.Catalog.Features.Categories.CreateCategory;

public sealed class CreateCategoryCommand
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}