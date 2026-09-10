using ECommerce.Modules.Orders.Application.Interfaces;
using ECommerce.Modules.Orders.Domain.Entities;
using ECommerce.Shared.Abstractions;
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

    public CancelOrderEndpoint(
        IOrderRepository orderRepository,
        ICurrentUser currentUser)
    {
        _orderRepository = orderRepository;
        _currentUser = currentUser;
    }

    [HttpPut("{orderId:guid}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, _currentUser.UserId);

        if (order is null)
        {
            return NotFound();
        }

        try
        {
            order.Cancel();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        await _orderRepository.UpdateStatusAsync(order.Id, _currentUser.UserId, OrderStatus.Cancelled);

        await _orderRepository.AddStatusHistoryAsync(new OrderStatusHistory(order.Id, OrderStatus.Cancelled));

        return NoContent();
    }
}