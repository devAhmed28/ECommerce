using ECommerce.Modules.Inventory.Application.DTOs;

namespace ECommerce.Modules.Inventory.Features.GetInventory;

public sealed class GetInventoryResponse
{
    public InventoryDto Inventory { get; init; } = null!;
}