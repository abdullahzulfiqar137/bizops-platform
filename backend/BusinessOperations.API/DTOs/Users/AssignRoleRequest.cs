namespace BusinessOperations.API.DTOs.Users;

public sealed class AssignRoleRequest
{
    public Guid UserId { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
}
