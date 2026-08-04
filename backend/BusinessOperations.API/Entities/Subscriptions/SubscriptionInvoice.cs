using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessOperations.API.Entities.Common;

namespace BusinessOperations.API.Entities.Subscriptions;

public enum SubscriptionInvoiceStatus
{
    Pending = 1,
    Paid = 2,
    Failed = 3
}

public class SubscriptionInvoice : BaseAuditableEntity
{
    [Required]
    public Guid SubscriptionId { get; set; }

    [ForeignKey(nameof(SubscriptionId))]
    public virtual TenantSubscription? Subscription { get; set; }

    [Required]
    [MaxLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    [MaxLength(10)]
    public string Currency { get; set; } = "USD";

    public SubscriptionInvoiceStatus Status { get; set; } = SubscriptionInvoiceStatus.Paid;

    public DateTime BillingDateUtc { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string PaymentTransactionReference { get; set; } = string.Empty;
}
