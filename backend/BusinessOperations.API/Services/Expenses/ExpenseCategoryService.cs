using BusinessOperations.API.DTOs.Expenses;
using BusinessOperations.API.Entities.Expenses;
using BusinessOperations.API.Interfaces.Repositories;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Services.Expenses;

public sealed class ExpenseCategoryService : IExpenseCategoryService
{
    private readonly IExpenseCategoryRepository _categoryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ExpenseCategoryService> _logger;

    public ExpenseCategoryService(
        IExpenseCategoryRepository categoryRepository,
        ICurrentUserService currentUserService,
        ILogger<ExpenseCategoryService> logger)
    {
        _categoryRepository = categoryRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<ExpenseCategoryResponse>>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        var response = categories.Select(MapToResponse);
        return ApiResponse<IEnumerable<ExpenseCategoryResponse>>.SuccessResponse(response, "Expense categories retrieved successfully.");
    }

    public async Task<ApiResponse<ExpenseCategoryResponse>> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            return ApiResponse<ExpenseCategoryResponse>.FailureResponse("Expense category not found.");
        }

        return ApiResponse<ExpenseCategoryResponse>.SuccessResponse(MapToResponse(category), "Expense category retrieved successfully.");
    }

    public async Task<ApiResponse<ExpenseCategoryResponse>> CreateCategoryAsync(CreateExpenseCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = new ExpenseCategory
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            IsActive = true,
            CreatedBy = _currentUserService.Username ?? "System",
            CreatedAtUtc = DateTime.UtcNow
        };

        await _categoryRepository.AddAsync(category, cancellationToken);
        _logger.LogInformation("Expense category created successfully. Id: {Id}, Name: {Name}", category.Id, category.Name);

        return ApiResponse<ExpenseCategoryResponse>.SuccessResponse(MapToResponse(category), "Expense category created successfully.");
    }

    public async Task<ApiResponse<ExpenseCategoryResponse>> UpdateCategoryAsync(Guid id, UpdateExpenseCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            return ApiResponse<ExpenseCategoryResponse>.FailureResponse("Expense category not found.");
        }

        category.Name = request.Name.Trim();
        category.Description = request.Description?.Trim();
        category.IsActive = request.IsActive;
        category.UpdatedBy = _currentUserService.Username;
        category.UpdatedAtUtc = DateTime.UtcNow;

        await _categoryRepository.UpdateAsync(category, cancellationToken);
        _logger.LogInformation("Expense category updated successfully. Id: {Id}", category.Id);

        return ApiResponse<ExpenseCategoryResponse>.SuccessResponse(MapToResponse(category), "Expense category updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            return ApiResponse<bool>.FailureResponse("Expense category not found.");
        }

        category.DeletedBy = _currentUserService.Username;
        await _categoryRepository.DeleteAsync(category, cancellationToken);
        _logger.LogInformation("Expense category deleted successfully. Id: {Id}", category.Id);

        return ApiResponse<bool>.SuccessResponse(true, "Expense category deleted successfully.");
    }

    private static ExpenseCategoryResponse MapToResponse(ExpenseCategory category)
    {
        return new ExpenseCategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            CreatedAtUtc = category.CreatedAtUtc
        };
    }
}
