using BusinessOperations.API.Entities.Common;
using BusinessOperations.API.Entities.Products;
using BusinessOperations.API.Enums;

namespace BusinessOperations.API.Entities.Inventory;

public sealed class InventoryMovement : BaseAuditableEntity
{
    public string MovementNumber { get; set; } = string.Empty;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public InventoryMovementType MovementType { get; set; }
    public decimal Quantity { get; set; }
    public decimal PreviousStock { get; set; }
    public decimal NewStock { get; set; }

    public string ReferenceType { get; set; } = string.Empty; // e.g. "Adjustment", "Sale", "Purchase"
    public Guid ReferenceId { get; set; }
    public string? Notes { get; set; }
}
