using ECommerce.Modules.Orders.Application.Contracts;
using ECommerce.Modules.Orders.Application.Interfaces;
using ECommerce.Modules.Orders.Infrastructure.Database;
using ECommerce.Modules.Orders.Infrastructure.Repositories;
using ECommerce.Modules.Orders.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Modules.Orders;

public static class DependencyInjection
{
    public static IServiceCollection AddOrdersModule(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IDbConnectionFactory>(
            _ => new SqlConnectionFactory(connectionString));

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderPaymentReader, OrderPaymentReader>();

        return services;
    }
}