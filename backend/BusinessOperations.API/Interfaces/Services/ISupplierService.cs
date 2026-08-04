using BusinessOperations.API.DTOs.Suppliers;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Interfaces.Services;

public interface ISupplierService
{
    Task<PagedResponse<SupplierResponse>> GetSuppliersAsync(
        string? searchTerm,
        string? city,
        bool? isActive,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<SupplierResponse>> GetSupplierByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<SupplierResponse>> CreateSupplierAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<SupplierResponse>> UpdateSupplierAsync(Guid id, UpdateSupplierRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteSupplierAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<decimal>> GetSupplierBalanceAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<SupplierPurchaseHistoryResponse>>> GetSupplierPurchasesAsync(Guid id, CancellationToken cancellationToken = default);
}
