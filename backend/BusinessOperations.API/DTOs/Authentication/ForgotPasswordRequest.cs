namespace BusinessOperations.API.DTOs.Authentication;

public sealed class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}
