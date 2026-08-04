using BusinessOperations.API.Entities.Common;
using BusinessOperations.API.Entities.Products;

namespace BusinessOperations.API.Entities.Purchases;

public sealed class PurchaseItem : BaseEntity
{
    public Guid PurchaseId { get; set; }
    public Purchase Purchase { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalPrice { get; set; }
}
