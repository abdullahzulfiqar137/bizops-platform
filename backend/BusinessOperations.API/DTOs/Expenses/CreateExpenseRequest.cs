using BusinessOperations.API.Enums;

namespace BusinessOperations.API.DTOs.Expenses;

public sealed class CreateExpenseRequest
{
    public Guid ExpenseCategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}
