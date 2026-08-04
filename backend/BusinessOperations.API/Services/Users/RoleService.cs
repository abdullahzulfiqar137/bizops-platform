using BusinessOperations.API.DTOs.Users;
using BusinessOperations.API.Entities.Identity;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Services.Users;

public sealed class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RoleService> _logger;

    public RoleService(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ILogger<RoleService> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<RoleResponse>>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _roleManager.Roles
            .AsNoTracking()
            .Select(r => new RoleResponse
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty,
                Description = r.Description,
                CreatedAtUtc = r.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<RoleResponse>>.SuccessResponse(roles, "Roles retrieved successfully.");
    }

    public async Task<ApiResponse<UserResponse>> AssignRolesToUserAsync(AssignRoleRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            return ApiResponse<UserResponse>.FailureResponse("User not found.");
        }

        foreach (var roleName in request.Roles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return ApiResponse<UserResponse>.FailureResponse($"Role '{roleName}' does not exist.");
            }
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
        {
            return ApiResponse<UserResponse>.FailureResponse("Failed to update user roles.");
        }

        var addResult = await _userManager.AddToRolesAsync(user, request.Roles);
        if (!addResult.Succeeded)
        {
            return ApiResponse<UserResponse>.FailureResponse("Failed to assign new roles to user.");
        }

        var updatedRoles = await _userManager.GetRolesAsync(user);
        var response = new UserResponse
        {
            Id = user.Id,
            TenantId = user.TenantId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            Username = user.UserName ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.IsActive,
            Roles = updatedRoles,
            CreatedAtUtc = user.CreatedAtUtc
        };

        _logger.LogInformation("Assigned roles to user. UserId: {UserId}, Roles: {Roles}", user.Id, string.Join(", ", request.Roles));

        return ApiResponse<UserResponse>.SuccessResponse(response, "Roles assigned to user successfully.");
    }
}
