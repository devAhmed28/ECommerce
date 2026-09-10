using ECommerce.Modules.Orders.Domain.Entities;

namespace ECommerce.Modules.Orders.Application.Interfaces;
public interface IOrderRepository
{
    Task CreateAsync(Order order, IReadOnlyList<OrderItem> items, OrderStatusHistory statusHistory, Guid userId);
    Task<Order?> GetByIdAsync(Guid orderId, Guid userId);
    Task<IReadOnlyList<Order>> GetByUserIdAsync(Guid userId);
    Task<IReadOnlyList<OrderItem>> GetItemsAsync(Guid orderId);
    Task<OrderStatusHistory?> GetLatestStatusHistoryAsync(Guid orderId);
    Task UpdateStatusAsync(Guid orderId, Guid userId, OrderStatus status);
    Task AddStatusHistoryAsync(OrderStatusHistory statusHistory);
}