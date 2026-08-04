using BusinessOperations.API.Entities.Common;

namespace BusinessOperations.API.Entities.Businesses;

public sealed class BusinessProfile : BaseAuditableEntity
{
    public string CompanyName { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string? TaxNumber { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = "USD";
    public string CurrencySymbol { get; set; } = "$";
    public decimal TaxRate { get; set; } = 0.00m;
    public string InvoicePrefix { get; set; } = "INV";
    public long NextInvoiceNumber { get; set; } = 1001;
    public string? LogoUrl { get; set; }
}
