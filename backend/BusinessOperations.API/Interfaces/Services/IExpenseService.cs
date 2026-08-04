using BusinessOperations.API.DTOs.Expenses;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Interfaces.Services;

public interface IExpenseService
{
    Task<ApiResponse<ExpenseResponse>> CreateExpenseAsync(CreateExpenseRequest request, CancellationToken cancellationToken = default);

    Task<PagedResponse<ExpenseResponse>> GetExpensesAsync(
        string? searchTerm,
        Guid? expenseCategoryId,
        DateTime? startDate,
        DateTime? endDate,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<ExpenseResponse>> GetExpenseByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<ExpenseResponse>> UpdateExpenseAsync(Guid id, UpdateExpenseRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteExpenseAsync(Guid id, CancellationToken cancellationToken = default);
}
