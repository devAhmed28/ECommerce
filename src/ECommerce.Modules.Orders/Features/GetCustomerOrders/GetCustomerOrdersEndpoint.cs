using ECommerce.Modules.Orders.Application.DTOs;
using ECommerce.Modules.Orders.Application.Interfaces;
using ECommerce.Shared.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Orders.Features.GetCustomerOrders;

[ApiController]
[Route("api/orders")]
[Authorize]
public sealed class GetCustomerOrdersEndpoint : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICurrentUser _currentUser;

    public GetCustomerOrdersEndpoint(
        IOrderRepository orderRepository,
        ICurrentUser currentUser)
    {
        _orderRepository = orderRepository;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomerOrders()
    {
        var orders = await _orderRepository.GetByUserIdAsync(_currentUser.UserId);

        var result = new List<OrderDto>();

        foreach (var order in orders)
        {
            var items = await _orderRepository.GetItemsAsync(order.Id);

            result.Add(new OrderDto
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
            });

        }
        return Ok(result);
    }
}