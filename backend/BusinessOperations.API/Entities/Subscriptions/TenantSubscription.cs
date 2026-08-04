using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessOperations.API.Entities.Common;

namespace BusinessOperations.API.Entities.Subscriptions;

public enum SubscriptionStatus
{
    Trialing = 1,
    Active = 2,
    PastDue = 3,
    Cancelled = 4,
    Expired = 5
}

public class TenantSubscription : BaseAuditableEntity
{
    [Required]
    public Guid PlanId { get; set; }

    [ForeignKey(nameof(PlanId))]
    public virtual SubscriptionPlan? Plan { get; set; }

    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Trialing;

    public DateTime StartDateUtc { get; set; } = DateTime.UtcNow;
    public DateTime? EndDateUtc { get; set; }
    public DateTime? TrialEndsAtUtc { get; set; } = DateTime.UtcNow.AddDays(14);
    public bool AutoRenew { get; set; } = true;

    [MaxLength(100)]
    public string PaymentMethod { get; set; } = "Credit Card";

    [MaxLength(100)]
    public string ExternalCustomerId { get; set; } = string.Empty;
}
