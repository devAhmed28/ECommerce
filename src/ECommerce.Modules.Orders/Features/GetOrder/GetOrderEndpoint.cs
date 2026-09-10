using ECommerce.Modules.Orders.Application.DTOs;
using ECommerce.Modules.Orders.Application.Interfaces;
using ECommerce.Shared.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Orders.Features.GetOrder;

[ApiController]
[Route("api/orders")]
[Authorize]
public sealed class GetOrderEndpoint : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICurrentUser _currentUser;

    public GetOrderEndpoint(IOrderRepository orderRepository, ICurrentUser currentUser)
    {
        _orderRepository = orderRepository;
        _currentUser = currentUser;
    }

    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetOrder(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, _currentUser.UserId);

        if (order is null)
        {
            return NotFound();
        }

        var items = await _orderRepository.GetItemsAsync(order.Id);

        var orderDto = new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = items
                .Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    LineTotal = item.LineTotal
                })
                .ToList()
        };

        return Ok(orderDto);
    }
}