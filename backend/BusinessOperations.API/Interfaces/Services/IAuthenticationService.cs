using BusinessOperations.API.DTOs.Authentication;
using BusinessOperations.API.DTOs.Users;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Interfaces.Services;

public interface IAuthenticationService
{
    Task<ApiResponse<UserResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request, string ipAddress, string? userAgent, CancellationToken cancellationToken = default);
    Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress, string? userAgent, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> RevokeTokenAsync(RevokeTokenRequest request, string ipAddress, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<UserResponse>> GetCurrentUserProfileAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<UserResponse>> UpdateCurrentUserProfileAsync(UpdateMyProfileRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<TwoFactorSetupResponse>> GetTwoFactorSetupAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> EnableTwoFactorAsync(VerifyTwoFactorRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DisableTwoFactorAsync(VerifyTwoFactorRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<IReadOnlyList<ActiveSessionResponse>>> GetActiveSessionsAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> RevokeSessionAsync(Guid sessionId, string ipAddress, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> RevokeAllSessionsAsync(string ipAddress, CancellationToken cancellationToken = default);
}
