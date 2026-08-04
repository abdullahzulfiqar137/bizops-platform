namespace BusinessOperations.API.DTOs.Users;

public sealed class UserResponse
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
    public DateTime CreatedAtUtc { get; set; }
}
