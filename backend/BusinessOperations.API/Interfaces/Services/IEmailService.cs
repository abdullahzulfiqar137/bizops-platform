namespace BusinessOperations.API.Interfaces.Services;

public interface IEmailService
{
    Task SendPasswordResetAsync(
        string email,
        string recipientName,
        string resetUrl,
        CancellationToken cancellationToken = default);
}
