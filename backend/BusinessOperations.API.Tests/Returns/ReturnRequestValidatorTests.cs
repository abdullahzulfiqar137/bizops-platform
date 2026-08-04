using BusinessOperations.API.DTOs.Purchases;
using BusinessOperations.API.DTOs.Sales;
using BusinessOperations.API.Validators.Returns;
using Xunit;

namespace BusinessOperations.API.Tests.Returns;

public sealed class ReturnRequestValidatorTests
{
    [Fact]
    public async Task SaleReturn_RequiresReasonAndPositiveItems()
    {
        var result = await new CreateSaleReturnRequestValidator().ValidateAsync(new CreateSaleReturnRequest
        {
            Reason = "",
            Items = [new() { SaleItemId = Guid.NewGuid(), Quantity = 0 }]
        });
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "Reason");
        Assert.Contains(result.Errors, x => x.PropertyName.EndsWith("Quantity"));
    }

    [Fact]
    public async Task SaleReturn_RejectsDuplicateInvoiceItems()
    {
        var id = Guid.NewGuid();
        var result = await new CreateSaleReturnRequestValidator().ValidateAsync(new CreateSaleReturnRequest
        {
            Reason = "Damaged",
            Items = [new() { SaleItemId = id, Quantity = 1 }, new() { SaleItemId = id, Quantity = 1 }]
        });
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task PurchaseReturn_AcceptsValidRequest()
    {
        var result = await new CreatePurchaseReturnRequestValidator().ValidateAsync(new CreatePurchaseReturnRequest
        {
            Reason = "Supplier discrepancy",
            Notes = "Approved",
            Items = [new() { PurchaseItemId = Guid.NewGuid(), Quantity = 2 }]
        });
        Assert.True(result.IsValid);
    }
}
