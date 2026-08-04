namespace BusinessOperations.API.DTOs.Businesses;

public sealed class BusinessProfileResponse
{
    public Guid Id { get; set; }
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
    public string CurrencyCode { get; set; } = string.Empty;
    public string CurrencySymbol { get; set; } = string.Empty;
    public decimal TaxRate { get; set; }
    public string InvoicePrefix { get; set; } = string.Empty;
    public long NextInvoiceNumber { get; set; }
    public string? LogoUrl { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
