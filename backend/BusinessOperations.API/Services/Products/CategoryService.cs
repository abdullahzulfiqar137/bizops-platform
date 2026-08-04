using BusinessOperations.API.DTOs.Products;
using BusinessOperations.API.Entities.Products;
using BusinessOperations.API.Interfaces.Repositories;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Services.Products;

public sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(
        ICategoryRepository categoryRepository,
        ICurrentUserService currentUserService,
        ILogger<CategoryService> logger)
    {
        _categoryRepository = categoryRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<CategoryResponse>>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        var responses = categories.Select(MapToResponse);
        return ApiResponse<IEnumerable<CategoryResponse>>.SuccessResponse(responses, "Categories retrieved successfully.");
    }

    public async Task<ApiResponse<CategoryResponse>> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            return ApiResponse<CategoryResponse>.FailureResponse("Category not found.");
        }

        return ApiResponse<CategoryResponse>.SuccessResponse(MapToResponse(category), "Category details retrieved successfully.");
    }

    public async Task<ApiResponse<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            IsActive = true,
            CreatedBy = _currentUserService.Username,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _categoryRepository.AddAsync(category, cancellationToken);
        _logger.LogInformation("Category created. Id: {Id}, Name: {Name}", category.Id, category.Name);

        return ApiResponse<CategoryResponse>.SuccessResponse(MapToResponse(category), "Category created successfully.");
    }

    public async Task<ApiResponse<CategoryResponse>> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            return ApiResponse<CategoryResponse>.FailureResponse("Category not found.");
        }

        category.Name = request.Name;
        category.Description = request.Description;
        category.IsActive = request.IsActive;
        category.UpdatedBy = _currentUserService.Username;
        category.UpdatedAtUtc = DateTime.UtcNow;

        await _categoryRepository.UpdateAsync(category, cancellationToken);
        _logger.LogInformation("Category updated. Id: {Id}", category.Id);

        return ApiResponse<CategoryResponse>.SuccessResponse(MapToResponse(category), "Category updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            return ApiResponse<bool>.FailureResponse("Category not found.");
        }

        category.DeletedBy = _currentUserService.Username;
        await _categoryRepository.DeleteAsync(category, cancellationToken);
        _logger.LogInformation("Category soft-deleted. Id: {Id}", category.Id);

        return ApiResponse<bool>.SuccessResponse(true, "Category deleted successfully.");
    }

    private static CategoryResponse MapToResponse(Category category)
    {
        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            CreatedAtUtc = category.CreatedAtUtc
        };
    }
}
