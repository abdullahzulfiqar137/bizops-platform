using BusinessOperations.API.Entities.Common;

namespace BusinessOperations.API.Entities.Products;

public sealed class Product : BaseAuditableEntity
{
    public string SKU { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public Guid UnitId { get; set; }
    public ProductUnit Unit { get; set; } = null!;

    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal StockQuantity { get; set; } = 0.00m;
    public decimal MinimumStockLevel { get; set; } = 5.00m;
    public bool IsActive { get; set; } = true;
}
