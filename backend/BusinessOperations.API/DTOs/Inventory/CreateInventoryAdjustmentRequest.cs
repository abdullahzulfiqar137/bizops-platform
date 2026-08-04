using BusinessOperations.API.Enums;

namespace BusinessOperations.API.DTOs.Inventory;

public sealed class CreateInventoryAdjustmentRequest
{
    public Guid ProductId { get; set; }
    public InventoryAdjustmentType AdjustmentType { get; set; }
    public decimal Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
