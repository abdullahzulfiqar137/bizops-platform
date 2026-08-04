using BusinessOperations.API.Entities.Common;
using BusinessOperations.API.Enums;

namespace BusinessOperations.API.Entities.Expenses;

public sealed class Expense : BaseAuditableEntity
{
    public string ExpenseNumber { get; set; } = string.Empty;

    public Guid ExpenseCategoryId { get; set; }
    public ExpenseCategory ExpenseCategory { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime ExpenseDateUtc { get; set; } = DateTime.UtcNow;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}
