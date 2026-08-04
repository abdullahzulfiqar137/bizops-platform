using BusinessOperations.API.DTOs.Products;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Interfaces.Services;

public interface ICategoryService
{
    Task<ApiResponse<IEnumerable<CategoryResponse>>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<CategoryResponse>> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<CategoryResponse>> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);
}
