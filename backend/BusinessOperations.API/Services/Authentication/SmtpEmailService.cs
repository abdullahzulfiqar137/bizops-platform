using System.Net;
using System.Net.Mail;
using BusinessOperations.API.Configurations;
using BusinessOperations.API.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace BusinessOperations.API.Services.Authentication;

public sealed class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(
        IOptions<EmailSettings> settings,
        ILogger<SmtpEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendPasswordResetAsync(
        string email,
        string recipientName,
        string resetUrl,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.Host) ||
            string.IsNullOrWhiteSpace(_settings.FromAddress))
        {
            throw new InvalidOperationException(
                "Password recovery email is not configured. Configure the Email SMTP settings.");
        }

        using var message = new MailMessage
        {
            From = new MailAddress(_settings.FromAddress, _settings.FromName),
            Subject = "Reset your BizOps password",
            IsBodyHtml = true,
            Body = $"""
                <div style="font-family:Arial,sans-serif;max-width:560px;margin:auto;color:#172033">
                  <h2>Reset your BizOps password</h2>
                  <p>Hello {WebUtility.HtmlEncode(recipientName)},</p>
                  <p>A password reset was requested for your account. Use the secure button below to choose a new password.</p>
                  <p style="margin:28px 0"><a href="{WebUtility.HtmlEncode(resetUrl)}" style="background:#2563eb;color:#fff;padding:12px 18px;border-radius:8px;text-decoration:none;font-weight:700">Reset password</a></p>
                  <p>If you did not request this, you can safely ignore this message. The link is single-use and protected by ASP.NET Identity.</p>
                </div>
                """
        };
        message.To.Add(email);

        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.EnableSsl,
            Credentials = string.IsNullOrWhiteSpace(_settings.Username)
                ? CredentialCache.DefaultNetworkCredentials
                : new NetworkCredential(_settings.Username, _settings.Password)
        };

        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            await client.SendMailAsync(message, cancellationToken);
            _logger.LogInformation("Password reset email delivered to SMTP server for {Email}.", email);
        }
        catch (SmtpException exception)
        {
            _logger.LogError(exception, "SMTP failed while sending a password reset email to {Email}.", email);
            throw new InvalidOperationException(
                "We found your account, but the reset email could not be sent. Please contact the administrator.",
                exception);
        }
    }
}
