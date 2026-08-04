using BusinessOperations.API.Entities.Common;
using BusinessOperations.API.Entities.Products;

namespace BusinessOperations.API.Entities.Sales;

public sealed class SaleReturnItem : BaseEntity
{
    public Guid SaleReturnId { get; set; }
    public SaleReturn SaleReturn { get; set; } = null!;
    public Guid SaleItemId { get; set; }
    public SaleItem SaleItem { get; set; } = null!;
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public bool Restock { get; set; }
}
