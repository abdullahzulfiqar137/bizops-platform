using System.Security.Claims;
using BusinessOperations.API.Entities.Identity;

namespace BusinessOperations.API.Interfaces.Services;

public interface ITokenGeneratorService
{
    Task<(string AccessToken, DateTime ExpiresAtUtc)> GenerateAccessTokenAsync(ApplicationUser user);
    RefreshToken GenerateRefreshToken(ApplicationUser user, string ipAddress);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
