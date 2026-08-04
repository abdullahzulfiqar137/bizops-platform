namespace BusinessOperations.API.DTOs.Customers;

public sealed class CustomerTransactionHistoryResponse
{
    public Guid TransactionId { get; set; }
    public string TransactionType { get; set; } = string.Empty; // e.g., Sale, Payment
    public string ReferenceNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public DateTime TransactionDateUtc { get; set; }
}
