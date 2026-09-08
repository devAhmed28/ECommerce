using ECommerce.Modules.Cart.Application.Interfaces;
using ECommerce.Modules.Cart.Features.AddItem;
using ECommerce.Modules.Cart.Infrastructure.Database;
using ECommerce.Modules.Cart.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Modules.Cart;

public static class DependencyInjection
{
    public static IServiceCollection AddCartModule(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IDbConnectionFactory>(
            _ => new SqlConnectionFactory(connectionString));

        services.AddScoped<ICartRepository, CartRepository>();

        services.AddValidatorsFromAssemblyContaining<AddItemRequestValidator>();

        return services;
    }
}