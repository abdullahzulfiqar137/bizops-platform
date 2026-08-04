namespace BusinessOperations.API.DTOs.Sales;

public sealed class CreateSaleReturnRequest
{
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<CreateSaleReturnItemRequest> Items { get; set; } = [];
}

public sealed class CreateSaleReturnItemRequest
{
    public Guid SaleItemId { get; set; }
    public decimal Quantity { get; set; }
    public bool Restock { get; set; } = true;
}
