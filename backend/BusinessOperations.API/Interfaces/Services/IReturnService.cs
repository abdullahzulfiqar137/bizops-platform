using BusinessOperations.API.DTOs.Purchases;
using BusinessOperations.API.DTOs.Sales;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Interfaces.Services;

public interface IReturnService
{
    Task<ApiResponse<SaleReturnResponse>> CreateSaleReturnAsync(Guid saleId, CreateSaleReturnRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<IReadOnlyList<SaleReturnResponse>>> GetSaleReturnsAsync(Guid saleId, CancellationToken cancellationToken = default);
    Task<ApiResponse<PurchaseReturnResponse>> CreatePurchaseReturnAsync(Guid purchaseId, CreatePurchaseReturnRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<IReadOnlyList<PurchaseReturnResponse>>> GetPurchaseReturnsAsync(Guid purchaseId, CancellationToken cancellationToken = default);
}
