namespace BusinessOperations.API.DTOs.Sales;

public sealed class CreateSaleItemRequest
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
