namespace BusinessOperations.API.DTOs.Authentication;

public sealed class RefreshTokenRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
