using BusinessOperations.API.Entities.Common;
using BusinessOperations.API.Entities.Products;

namespace BusinessOperations.API.Entities.Sales;

public sealed class SaleItem : BaseEntity
{
    public Guid SaleId { get; set; }
    public Sale Sale { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
