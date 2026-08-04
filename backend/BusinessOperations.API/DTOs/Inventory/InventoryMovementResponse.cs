using BusinessOperations.API.Enums;

namespace BusinessOperations.API.DTOs.Inventory;

public sealed class InventoryMovementResponse
{
    public Guid Id { get; set; }
    public string MovementNumber { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public InventoryMovementType MovementType { get; set; }
    public string MovementTypeDescription => MovementType.ToString();
    public decimal Quantity { get; set; }
    public decimal PreviousStock { get; set; }
    public decimal NewStock { get; set; }
    public string ReferenceType { get; set; } = string.Empty;
    public Guid ReferenceId { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
