using BusinessOperations.API.DTOs.Users;
using BusinessOperations.API.Entities.Identity;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Services.Users;

public sealed class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserService> _logger;

    public UserService(UserManager<ApplicationUser> userManager, ILogger<UserService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<PagedResponse<UserResponse>> GetUsersAsync(
        string? searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

        var query = _userManager.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim().ToLower();
            query = query.Where(u =>
                u.FirstName.ToLower().Contains(search) ||
                u.LastName.ToLower().Contains(search) ||
                (u.Email != null && u.Email.ToLower().Contains(search)) ||
                (u.UserName != null && u.UserName.ToLower().Contains(search)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .OrderByDescending(u => u.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var userResponses = new List<UserResponse>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userResponses.Add(new UserResponse
            {
                Id = user.Id,
                TenantId = user.TenantId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Username = user.UserName ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                Roles = roles,
                CreatedAtUtc = user.CreatedAtUtc
            });
        }

        return new PagedResponse<UserResponse>(userResponses, totalCount, pageNumber, pageSize, "Users retrieved successfully.");
    }

    public async Task<ApiResponse<UserResponse>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return ApiResponse<UserResponse>.FailureResponse("User not found.");
        }

        var roles = await _userManager.GetRolesAsync(user);
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
            Roles = roles,
            CreatedAtUtc = user.CreatedAtUtc
        };

        return ApiResponse<UserResponse>.SuccessResponse(response, "User profile retrieved successfully.");
    }

    public async Task<ApiResponse<UserResponse>> UpdateUserStatusAsync(Guid userId, bool isActive, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return ApiResponse<UserResponse>.FailureResponse("User not found.");
        }

        user.IsActive = isActive;
        user.UpdatedAtUtc = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return ApiResponse<UserResponse>.FailureResponse("Failed to update user status.");
        }

        var roles = await _userManager.GetRolesAsync(user);
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
            Roles = roles,
            CreatedAtUtc = user.CreatedAtUtc
        };

        _logger.LogInformation("Updated user active status. UserId: {UserId}, IsActive: {IsActive}", user.Id, isActive);

        return ApiResponse<UserResponse>.SuccessResponse(response, $"User active status set to {isActive}.");
    }
}
