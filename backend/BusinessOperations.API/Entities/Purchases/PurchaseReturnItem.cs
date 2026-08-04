using BusinessOperations.API.Entities.Common;
using BusinessOperations.API.Entities.Products;

namespace BusinessOperations.API.Entities.Purchases;

public sealed class PurchaseReturnItem : BaseEntity
{
    public Guid PurchaseReturnId { get; set; }
    public PurchaseReturn PurchaseReturn { get; set; } = null!;
    public Guid PurchaseItemId { get; set; }
    public PurchaseItem PurchaseItem { get; set; } = null!;
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalPrice { get; set; }
}
