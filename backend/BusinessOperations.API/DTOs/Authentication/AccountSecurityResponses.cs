namespace BusinessOperations.API.DTOs.Authentication;

public sealed class TwoFactorSetupResponse
{
    public bool IsEnabled { get; set; }
    public string SharedKey { get; set; } = string.Empty;
    public string AuthenticatorUri { get; set; } = string.Empty;
}

public sealed class VerifyTwoFactorRequest
{
    public string Code { get; set; } = string.Empty;
}

public sealed class ActiveSessionResponse
{
    public Guid Id { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string Device { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
}
