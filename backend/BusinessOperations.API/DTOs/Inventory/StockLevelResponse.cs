namespace BusinessOperations.API.DTOs.Inventory;

public sealed class StockLevelResponse
{
    public Guid ProductId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string UnitAbbreviation { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public decimal MinimumStockLevel { get; set; }
    public bool IsLowStock => CurrentStock <= MinimumStockLevel;
}
