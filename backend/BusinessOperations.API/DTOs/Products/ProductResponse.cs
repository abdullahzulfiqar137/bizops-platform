namespace BusinessOperations.API.DTOs.Products;

public sealed class ProductResponse
{
    public Guid Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public Guid UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public string UnitAbbreviation { get; set; } = string.Empty;

    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal StockQuantity { get; set; }
    public decimal MinimumStockLevel { get; set; }
    public bool IsLowStock => StockQuantity <= MinimumStockLevel;
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
