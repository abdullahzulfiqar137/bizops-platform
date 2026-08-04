using BusinessOperations.API.DTOs.Expenses;
using BusinessOperations.API.Entities.Expenses;
using BusinessOperations.API.Interfaces.Repositories;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Services.Expenses;

public sealed class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IExpenseCategoryRepository _categoryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ExpenseService> _logger;

    public ExpenseService(
        IExpenseRepository expenseRepository,
        IExpenseCategoryRepository categoryRepository,
        ICurrentUserService currentUserService,
        ILogger<ExpenseService> logger)
    {
        _expenseRepository = expenseRepository;
        _categoryRepository = categoryRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<ApiResponse<ExpenseResponse>> CreateExpenseAsync(CreateExpenseRequest request, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(request.ExpenseCategoryId, cancellationToken);
        if (category == null)
        {
            return ApiResponse<ExpenseResponse>.FailureResponse("Expense category not found.");
        }

        var expenseNumber = await _expenseRepository.GenerateNextExpenseNumberAsync(cancellationToken);

        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            ExpenseNumber = expenseNumber,
            ExpenseCategoryId = category.Id,
            Title = request.Title.Trim(),
            Amount = request.Amount,
            ExpenseDateUtc = DateTime.UtcNow,
            PaymentMethod = request.PaymentMethod,
            ReferenceNumber = request.ReferenceNumber?.Trim(),
            Notes = request.Notes?.Trim(),
            CreatedBy = _currentUserService.Username ?? "System",
            CreatedAtUtc = DateTime.UtcNow
        };

        await _expenseRepository.AddAsync(expense, cancellationToken);
        _logger.LogInformation("Expense created successfully. ExpenseNo: {ExpenseNo}, Amount: {Amount}", expense.ExpenseNumber, expense.Amount);

        var createdExpense = await _expenseRepository.GetExpenseWithCategoryAsync(expense.Id, cancellationToken);
        return ApiResponse<ExpenseResponse>.SuccessResponse(MapToResponse(createdExpense!), "Expense created successfully.");
    }

    public async Task<PagedResponse<ExpenseResponse>> GetExpensesAsync(
        string? searchTerm,
        Guid? expenseCategoryId,
        DateTime? startDate,
        DateTime? endDate,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

        var (items, totalCount) = await _expenseRepository.GetPagedExpensesAsync(
            searchTerm, expenseCategoryId, startDate, endDate, sortBy, isDescending, pageNumber, pageSize, cancellationToken);

        var responses = items.Select(MapToResponse);

        return new PagedResponse<ExpenseResponse>(responses, totalCount, pageNumber, pageSize, "Expenses retrieved successfully.");
    }

    public async Task<ApiResponse<ExpenseResponse>> GetExpenseByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var expense = await _expenseRepository.GetExpenseWithCategoryAsync(id, cancellationToken);
        if (expense == null)
        {
            return ApiResponse<ExpenseResponse>.FailureResponse("Expense record not found.");
        }

        return ApiResponse<ExpenseResponse>.SuccessResponse(MapToResponse(expense), "Expense details retrieved successfully.");
    }

    public async Task<ApiResponse<ExpenseResponse>> UpdateExpenseAsync(Guid id, UpdateExpenseRequest request, CancellationToken cancellationToken = default)
    {
        var expense = await _expenseRepository.GetExpenseWithCategoryAsync(id, cancellationToken);
        if (expense == null)
        {
            return ApiResponse<ExpenseResponse>.FailureResponse("Expense record not found.");
        }

        var category = await _categoryRepository.GetByIdAsync(request.ExpenseCategoryId, cancellationToken);
        if (category == null)
        {
            return ApiResponse<ExpenseResponse>.FailureResponse("Expense category not found.");
        }

        expense.ExpenseCategoryId = category.Id;
        expense.Title = request.Title.Trim();
        expense.Amount = request.Amount;
        expense.PaymentMethod = request.PaymentMethod;
        expense.ReferenceNumber = request.ReferenceNumber?.Trim();
        expense.Notes = request.Notes?.Trim();
        expense.UpdatedBy = _currentUserService.Username;
        expense.UpdatedAtUtc = DateTime.UtcNow;

        await _expenseRepository.UpdateAsync(expense, cancellationToken);
        _logger.LogInformation("Expense updated successfully. Id: {Id}", expense.Id);

        var updatedExpense = await _expenseRepository.GetExpenseWithCategoryAsync(expense.Id, cancellationToken);
        return ApiResponse<ExpenseResponse>.SuccessResponse(MapToResponse(updatedExpense!), "Expense updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteExpenseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var expense = await _expenseRepository.GetByIdAsync(id, cancellationToken);
        if (expense == null)
        {
            return ApiResponse<bool>.FailureResponse("Expense record not found.");
        }

        expense.DeletedBy = _currentUserService.Username;
        await _expenseRepository.DeleteAsync(expense, cancellationToken);
        _logger.LogInformation("Expense deleted successfully. Id: {Id}", expense.Id);

        return ApiResponse<bool>.SuccessResponse(true, "Expense deleted successfully.");
    }

    private static ExpenseResponse MapToResponse(Expense expense)
    {
        return new ExpenseResponse
        {
            Id = expense.Id,
            ExpenseNumber = expense.ExpenseNumber,
            ExpenseCategoryId = expense.ExpenseCategoryId,
            ExpenseCategoryName = expense.ExpenseCategory?.Name ?? string.Empty,
            Title = expense.Title,
            Amount = expense.Amount,
            ExpenseDateUtc = expense.ExpenseDateUtc,
            PaymentMethod = expense.PaymentMethod,
            ReferenceNumber = expense.ReferenceNumber,
            Notes = expense.Notes,
            CreatedBy = expense.CreatedBy ?? string.Empty,
            CreatedAtUtc = expense.CreatedAtUtc
        };
    }
}
