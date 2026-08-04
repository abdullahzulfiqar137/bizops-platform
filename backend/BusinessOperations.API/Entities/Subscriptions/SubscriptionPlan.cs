using System.ComponentModel.DataAnnotations;
using BusinessOperations.API.Entities.Common;

namespace BusinessOperations.API.Entities.Subscriptions;

public class SubscriptionPlan : BaseAuditableEntity, IGlobalEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    public decimal MonthlyPrice { get; set; }
    public decimal AnnualPrice { get; set; }
    public int MaxUsers { get; set; }
    public int MaxSalesPerMonth { get; set; }
    public bool EnableAiAssistant { get; set; }
    public bool EnableDoubleEntryAccounting { get; set; }
    public bool EnablePdfGeneration { get; set; }
    public bool IsActive { get; set; } = true;
    public string Description { get; set; } = string.Empty;
}
