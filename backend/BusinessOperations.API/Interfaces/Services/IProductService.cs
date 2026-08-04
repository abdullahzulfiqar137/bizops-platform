using BusinessOperations.API.DTOs.Products;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Interfaces.Services;

public interface IProductService
{
    Task<PagedResponse<ProductResponse>> GetProductsAsync(
        string? searchTerm,
        Guid? categoryId,
        bool? isActive,
        bool? isLowStock,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<ProductResponse>> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<ProductResponse>> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<ProductResponse>> UpdateProductAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<ProductUnitResponse>>> GetProductUnitsAsync(CancellationToken cancellationToken = default);
}
