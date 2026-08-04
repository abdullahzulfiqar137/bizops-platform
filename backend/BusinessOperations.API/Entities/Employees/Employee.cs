using BusinessOperations.API.Entities.Common;

namespace BusinessOperations.API.Entities.Employees;

public sealed class Employee : BaseAuditableEntity
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public DateTime HireDateUtc { get; set; } = DateTime.UtcNow;
    public decimal MonthlySalary { get; set; }
    public string Status { get; set; } = "Active";
    public string AvatarColor { get; set; } = "#2563eb";
}
