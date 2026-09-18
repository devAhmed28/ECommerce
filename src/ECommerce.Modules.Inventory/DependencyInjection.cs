using ECommerce.Modules.Inventory.Application.Interfaces;
using ECommerce.Modules.Inventory.Features.AdjustStock;
using ECommerce.Modules.Inventory.Infrastructure.Database;
using ECommerce.Modules.Inventory.Infrastructure.Services;
using ECommerce.Shared.Abstractions.Inventory;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Modules.Inventory;

public static class ModuleInitializer
{
    public static IServiceCollection AddInventoryModule(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(new SqlConnectionFactory(connectionString));

        services.AddScoped<InventoryService>();

        services.AddScoped<IInventoryService>(serviceProvider =>
                serviceProvider.GetRequiredService<InventoryService>());

        services.AddScoped<IInventoryStockWriter>(serviceProvider =>
                serviceProvider.GetRequiredService<InventoryService>());

        services.AddValidatorsFromAssemblyContaining<AdjustStockValidator>();

        return services;
    }
}