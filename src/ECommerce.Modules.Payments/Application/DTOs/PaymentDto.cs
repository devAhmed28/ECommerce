using ECommerce.Modules.Payments.Domain.Entities;

namespace ECommerce.Modules.Payments.Application.DTOs;

public sealed record PaymentDto(
    Guid Id,
    Guid OrderId,
    decimal Amount,
    string Currency,
    PaymentStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt
);