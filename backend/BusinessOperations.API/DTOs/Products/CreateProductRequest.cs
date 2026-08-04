namespace BusinessOperations.API.DTOs.Products;

public sealed class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public Guid UnitId { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal InitialStockQuantity { get; set; } = 0.00m;
    public decimal MinimumStockLevel { get; set; } = 5.00m;
}
