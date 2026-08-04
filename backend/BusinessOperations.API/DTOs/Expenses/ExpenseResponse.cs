using BusinessOperations.API.Enums;

namespace BusinessOperations.API.DTOs.Expenses;

public sealed class ExpenseResponse
{
    public Guid Id { get; set; }
    public string ExpenseNumber { get; set; } = string.Empty;
    public Guid ExpenseCategoryId { get; set; }
    public string ExpenseCategoryName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime ExpenseDateUtc { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string PaymentMethodDescription => PaymentMethod.ToString();
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
