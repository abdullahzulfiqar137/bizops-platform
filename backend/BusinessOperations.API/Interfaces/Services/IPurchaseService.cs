using BusinessOperations.API.DTOs.Purchases;
using BusinessOperations.API.Enums;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Interfaces.Services;

public interface IPurchaseService
{
    Task<ApiResponse<PurchaseResponse>> CreatePurchaseAsync(CreatePurchaseRequest request, CancellationToken cancellationToken = default);

    Task<PagedResponse<PurchaseResponse>> GetPurchasesAsync(
        string? searchTerm,
        Guid? supplierId,
        PurchaseStatus? status,
        PaymentStatus? paymentStatus,
        DateTime? startDate,
        DateTime? endDate,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<PurchaseResponse>> GetPurchaseByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> CancelPurchaseAsync(Guid id, CancellationToken cancellationToken = default);
}
