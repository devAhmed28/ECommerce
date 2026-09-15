namespace ECommerce.Modules.Orders.Application.Contracts;

public interface IOrderPaymentReader
{
    Task<PayableOrder?> GetPayableOrderAsync(Guid orderId, Guid userId, CancellationToken cancellationToken);
}

public sealed record PayableOrder(Guid OrderId, Guid UserId, decimal TotalAmount);