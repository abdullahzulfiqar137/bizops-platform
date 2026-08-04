namespace BusinessOperations.API.Interfaces.Services;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? TenantId { get; }
    string? Username { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    string? IpAddress { get; }
}
