using BusinessOperations.API.DTOs.Customers;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Interfaces.Services;

public interface ICustomerService
{
    Task<PagedResponse<CustomerResponse>> GetCustomersAsync(
        string? searchTerm,
        string? city,
        bool? isActive,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<CustomerResponse>> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<CustomerResponse>> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<CustomerResponse>> UpdateCustomerAsync(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteCustomerAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<decimal>> GetCustomerBalanceAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<CustomerTransactionHistoryResponse>>> GetCustomerTransactionsAsync(Guid id, CancellationToken cancellationToken = default);
}
