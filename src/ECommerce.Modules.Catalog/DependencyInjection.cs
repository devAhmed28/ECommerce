using ECommerce.Modules.Catalog.Features.Categories.CreateCategory;
using ECommerce.Modules.Catalog.Features.Categories.DeleteCategory;
using ECommerce.Modules.Catalog.Features.Categories.GetCategories;
using ECommerce.Modules.Catalog.Features.Categories.GetCategory;
using ECommerce.Modules.Catalog.Features.Categories.UpdateCategory;
using ECommerce.Modules.Catalog.Features.CreateProduct;
using ECommerce.Modules.Catalog.Features.GetProduct;
using ECommerce.Modules.Catalog.Features.Products.DeleteProduct;
using ECommerce.Modules.Catalog.Features.Products.SearchProducts;
using ECommerce.Modules.Catalog.Features.Products.UpdateProduct;
using ECommerce.Modules.Catalog.Infrastructure.Database;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Modules.Catalog;
public static class DependencyInjection
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(new SqlConnectionFactory(connectionString));

        // Categories
        services.AddScoped<CreateCategoryHandler>();
        services.AddScoped<GetCategoryHandler>();
        services.AddScoped<GetCategoriesHandler>();
        services.AddScoped<UpdateCategoryHandler>();
        services.AddScoped<DeleteCategoryHandler>();

        // Products
        services.AddScoped<CreateProductHandler>();
        services.AddScoped<GetProductHandler>();
        services.AddScoped<SearchProductsHandler>();
        services.AddScoped<UpdateProductHandler>();
        services.AddScoped<DeleteProductHandler>();

        services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();

        return services;
    }
}
