using BusinessOperations.API.DTOs.Authentication;
using BusinessOperations.API.Validators.Authentication;
using Xunit;

namespace BusinessOperations.API.Tests.Authentication;

public sealed class PasswordRecoveryValidatorTests
{
    [Fact]
    public void ForgotPassword_RejectsInvalidEmail()
    {
        var result = new ForgotPasswordRequestValidator().Validate(
            new ForgotPasswordRequest { Email = "not-an-email" });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Email");
    }

    [Fact]
    public void ForgotPassword_AcceptsValidEmail()
    {
        var result = new ForgotPasswordRequestValidator().Validate(
            new ForgotPasswordRequest { Email = "owner@example.com" });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ResetPassword_RejectsMismatchedConfirmation()
    {
        var result = new ResetPasswordRequestValidator().Validate(
            new ResetPasswordRequest
            {
                Email = "owner@example.com",
                Token = "encoded-token",
                NewPassword = "Secure123!",
                ConfirmPassword = "Different123!"
            });

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == "ConfirmPassword");
    }

    [Fact]
    public void ResetPassword_AcceptsMatchingSecureInput()
    {
        var result = new ResetPasswordRequestValidator().Validate(
            new ResetPasswordRequest
            {
                Email = "owner@example.com",
                Token = "encoded-token",
                NewPassword = "Secure123!",
                ConfirmPassword = "Secure123!"
            });

        Assert.True(result.IsValid);
    }
}
