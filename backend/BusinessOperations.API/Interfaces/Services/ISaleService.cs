using BusinessOperations.API.DTOs.Sales;
using BusinessOperations.API.Entities.Sales;
using BusinessOperations.API.Enums;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Interfaces.Services;

public interface ISaleService
{
    Task<ApiResponse<SaleResponse>> CreateSaleAsync(CreateSaleRequest request, CancellationToken cancellationToken = default);

    Task<PagedResponse<SaleResponse>> GetSalesAsync(
        string? searchTerm,
        Guid? customerId,
        SaleStatus? status,
        PaymentStatus? paymentStatus,
        DateTime? startDate,
        DateTime? endDate,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<SaleResponse>> GetSaleByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Sale?> GetSaleEntityByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> CancelSaleAsync(Guid id, CancellationToken cancellationToken = default);
}
