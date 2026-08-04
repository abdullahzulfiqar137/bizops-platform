using BusinessOperations.API.Data;
using BusinessOperations.API.Entities.Expenses;
using BusinessOperations.API.Interfaces.Repositories;

namespace BusinessOperations.API.Repositories;

public sealed class ExpenseCategoryRepository : GenericRepository<ExpenseCategory>, IExpenseCategoryRepository
{
    public ExpenseCategoryRepository(ApplicationDbContext context) : base(context)
    {
    }
}
