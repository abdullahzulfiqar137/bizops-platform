using BusinessOperations.API.Entities.Expenses;

namespace BusinessOperations.API.Interfaces.Repositories;

public interface IExpenseRepository : IGenericRepository<Expense>
{
    Task<(IEnumerable<Expense> Items, int TotalCount)> GetPagedExpensesAsync(
        string? searchTerm,
        Guid? expenseCategoryId,
        DateTime? startDate,
        DateTime? endDate,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Expense?> GetExpenseWithCategoryAsync(Guid id, CancellationToken cancellationToken = default);
    Task<string> GenerateNextExpenseNumberAsync(CancellationToken cancellationToken = default);
}
