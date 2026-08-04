using BusinessOperations.API.DTOs.Users;
using System.Text.Json.Serialization;

namespace BusinessOperations.API.DTOs.Authentication;

public sealed class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    [JsonIgnore]
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAtUtc { get; set; }
    public DateTime RefreshTokenExpiresAtUtc { get; set; }
    public UserResponse User { get; set; } = null!;
}
