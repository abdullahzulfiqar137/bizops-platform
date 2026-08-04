using BusinessOperations.API.DTOs.Expenses;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Interfaces.Services;

public interface IExpenseCategoryService
{
    Task<ApiResponse<IEnumerable<ExpenseCategoryResponse>>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<ExpenseCategoryResponse>> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<ExpenseCategoryResponse>> CreateCategoryAsync(CreateExpenseCategoryRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<ExpenseCategoryResponse>> UpdateCategoryAsync(Guid id, UpdateExpenseCategoryRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);
}
