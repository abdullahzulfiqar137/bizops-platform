using BusinessOperations.API.DTOs.Users;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Interfaces.Services;

public interface IUserService
{
    Task<PagedResponse<UserResponse>> GetUsersAsync(
        string? searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<UserResponse>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ApiResponse<UserResponse>> UpdateUserStatusAsync(Guid userId, bool isActive, CancellationToken cancellationToken = default);
}
