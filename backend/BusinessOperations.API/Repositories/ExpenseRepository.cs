using BusinessOperations.API.Data;
using BusinessOperations.API.Entities.Expenses;
using BusinessOperations.API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Repositories;

public sealed class ExpenseRepository : GenericRepository<Expense>, IExpenseRepository
{
    public ExpenseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Expense> Items, int TotalCount)> GetPagedExpensesAsync(
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
        var query = DbSet
            .Include(e => e.ExpenseCategory)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim().ToLower();
            query = query.Where(e =>
                e.ExpenseNumber.ToLower().Contains(search) ||
                e.Title.ToLower().Contains(search) ||
                (e.ReferenceNumber != null && e.ReferenceNumber.ToLower().Contains(search)));
        }

        if (expenseCategoryId.HasValue)
        {
            query = query.Where(e => e.ExpenseCategoryId == expenseCategoryId.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(e => e.ExpenseDateUtc >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(e => e.ExpenseDateUtc <= endDate.Value);
        }

        query = sortBy?.ToLower() switch
        {
            "title" => isDescending ? query.OrderByDescending(e => e.Title) : query.OrderBy(e => e.Title),
            "amount" => isDescending ? query.OrderByDescending(e => e.Amount) : query.OrderBy(e => e.Amount),
            "expensedate" => isDescending ? query.OrderByDescending(e => e.ExpenseDateUtc) : query.OrderBy(e => e.ExpenseDateUtc),
            _ => isDescending ? query.OrderByDescending(e => e.CreatedAtUtc) : query.OrderBy(e => e.CreatedAtUtc)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Expense?> GetExpenseWithCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.ExpenseCategory)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<string> GenerateNextExpenseNumberAsync(CancellationToken cancellationToken = default)
    {
        var count = await DbSet.IgnoreQueryFilters()
            .CountAsync(x => x.TenantId == Context.CurrentTenantId, cancellationToken);
        return $"EXP-{(count + 1001):D6}";
    }
}
