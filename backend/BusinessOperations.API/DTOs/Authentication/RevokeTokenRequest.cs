namespace BusinessOperations.API.DTOs.Authentication;

public sealed class RevokeTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
