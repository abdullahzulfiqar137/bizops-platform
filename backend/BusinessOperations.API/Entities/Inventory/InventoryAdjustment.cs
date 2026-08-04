using BusinessOperations.API.Entities.Common;
using BusinessOperations.API.Entities.Products;
using BusinessOperations.API.Enums;

namespace BusinessOperations.API.Entities.Inventory;

public sealed class InventoryAdjustment : BaseAuditableEntity
{
    public string AdjustmentNumber { get; set; } = string.Empty;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public InventoryAdjustmentType AdjustmentType { get; set; }
    public decimal Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
