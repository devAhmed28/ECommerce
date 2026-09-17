using ECommerce.Modules.Inventory.Application.DTOs;

namespace ECommerce.Modules.Inventory.Application.Interfaces;

public interface IInventoryService
{
    Task<InventoryDto?> GetAsync(int productId, CancellationToken cancellationToken = default);

    Task AdjustAsync(int productId, int quantity, CancellationToken cancellationToken = default);

    Task ReserveAsync(int productId, Guid orderId, int quantity, CancellationToken cancellationToken = default);

    Task ReleaseAsync(int productId, Guid orderId, int quantity, CancellationToken cancellationToken = default);
}