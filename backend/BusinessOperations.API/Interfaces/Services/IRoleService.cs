using BusinessOperations.API.DTOs.Users;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Interfaces.Services;

public interface IRoleService
{
    Task<ApiResponse<IEnumerable<RoleResponse>>> GetAllRolesAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<UserResponse>> AssignRolesToUserAsync(AssignRoleRequest request, CancellationToken cancellationToken = default);
}
