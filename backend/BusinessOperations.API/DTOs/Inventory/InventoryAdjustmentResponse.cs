using BusinessOperations.API.Enums;

namespace BusinessOperations.API.DTOs.Inventory;

public sealed class InventoryAdjustmentResponse
{
    public Guid Id { get; set; }
    public string AdjustmentNumber { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public InventoryAdjustmentType AdjustmentType { get; set; }
    public string AdjustmentTypeDescription => AdjustmentType.ToString();
    public decimal Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
