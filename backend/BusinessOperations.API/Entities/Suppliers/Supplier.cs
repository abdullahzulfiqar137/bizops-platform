using BusinessOperations.API.Entities.Common;

namespace BusinessOperations.API.Entities.Suppliers;

public sealed class Supplier : BaseAuditableEntity
{
    public string SupplierCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public decimal CurrentBalance { get; set; } = 0.00m;
    public bool IsActive { get; set; } = true;
}
