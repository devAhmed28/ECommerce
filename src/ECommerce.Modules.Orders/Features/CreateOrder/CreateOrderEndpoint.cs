using Dapper;
using ECommerce.Modules.Orders.Application.DTOs;
using ECommerce.Modules.Orders.Application.Interfaces;
using ECommerce.Modules.Orders.Domain.Entities;
using ECommerce.Modules.Orders.Infrastructure.Database;
using ECommerce.Shared.Abstractions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Orders.Features.CreateOrder;

[ApiController]
[Route("api/orders")]
[Authorize]
public sealed class CreateOrderEndpoint : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ICurrentUser _currentUser;
    private readonly IValidator<CreateOrderRequest> _validator;

    public CreateOrderEndpoint(
        IOrderRepository orderRepository,
        IDbConnectionFactory dbConnectionFactory,
        ICurrentUser currentUser,
        IValidator<CreateOrderRequest> validator)
    {
        _orderRepository = orderRepository;
        _dbConnectionFactory = dbConnectionFactory;
        _currentUser = currentUser;
        _validator = validator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        const string sql = @"
            SELECT
                ci.ProductId,
                ci.Quantity,
                p.Name AS ProductName,
                p.Price
            FROM CartItems ci
            INNER JOIN Carts c
                ON c.Id = ci.CartId
            INNER JOIN Products p
                ON p.Id = ci.ProductId
            WHERE c.UserId = @UserId;
        ";

        using var connection = _dbConnectionFactory.CreateConnection();

        var cartItems = (await connection.QueryAsync<CartProductRow>(sql, new
        {
            UserId = _currentUser.UserId
        })).AsList();

        if (cartItems.Count == 0)
        {
            return BadRequest("Cart is empty.");
        }

        var totalAmount = cartItems.Sum(item => item.Price * item.Quantity);

        var order = new Order(_currentUser.UserId, totalAmount);

        var orderItems = cartItems
            .Select(item => new OrderItem(
                order.Id,
                item.ProductId,
                item.ProductName,
                item.Price,
                item.Quantity))
            .ToList();

        var statusHistory = new OrderStatusHistory(order.Id, OrderStatus.Pending);

        await _orderRepository.CreateAsync(order, orderItems, statusHistory, _currentUser.UserId);

        var orderDto = new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = orderItems
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

        return Created($"/api/orders/{order.Id}", orderDto);
    }

    private sealed class CartProductRow
    {
        public int ProductId { get; init; }
        public int Quantity { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public decimal Price { get; init; }
    }
}