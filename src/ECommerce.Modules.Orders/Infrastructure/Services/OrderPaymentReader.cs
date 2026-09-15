using ECommerce.Modules.Orders.Application.Contracts;
using ECommerce.Modules.Orders.Application.Interfaces;
using ECommerce.Modules.Orders.Domain.Entities;

namespace ECommerce.Modules.Orders.Infrastructure.Services;

public sealed class OrderPaymentReader(
    IOrderRepository orderRepository) : IOrderPaymentReader
{
    public async Task<PayableOrder?> GetPayableOrderAsync(Guid orderId, Guid userId, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(orderId, userId);

        if (order is null || order.UserId != userId || order.Status != OrderStatus.Pending)
        {
            return null;
        }

        return new PayableOrder(order.Id, order.UserId, order.TotalAmount);
    }
}