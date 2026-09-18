using ECommerce.Modules.Orders.Application.Interfaces;
using ECommerce.Modules.Orders.Domain.Entities;
using ECommerce.Shared.Abstractions;
using ECommerce.Shared.Abstractions.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Orders.Features.CancelOrder;

[ApiController]
[Route("api/orders")]
[Authorize]
public sealed class CancelOrderEndpoint : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IInventoryStockWriter _inventoryStockWriter;

    public CancelOrderEndpoint(IOrderRepository orderRepository, ICurrentUser currentUser, IInventoryStockWriter inventoryStockWriter)
    {
        _orderRepository = orderRepository;
        _currentUser = currentUser;
        _inventoryStockWriter = inventoryStockWriter;
    }

    [HttpPut("{orderId:guid}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, _currentUser.UserId);

        if (order is null)
            return NotFound();

        IReadOnlyList<OrderItem> orderItems;

        try
        {
            order.Cancel();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        orderItems = await _orderRepository.GetItemsAsync(orderId);

        var reservationItems = orderItems
            .Select(item => new InventoryReservationItem(item.ProductId, item.Quantity))
            .ToList();

        try
        {
            await _inventoryStockWriter.ReleaseAsync(order.Id, reservationItems, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                error = ex.Message
            });
        }

        await _orderRepository.UpdateStatusAsync(order.Id, _currentUser.UserId, OrderStatus.Cancelled);

        await _orderRepository.AddStatusHistoryAsync(new OrderStatusHistory(order.Id, OrderStatus.Cancelled));

        return NoContent();
    }
}