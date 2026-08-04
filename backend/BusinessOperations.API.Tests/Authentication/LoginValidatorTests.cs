using BusinessOperations.API.DTOs.Authentication;
using BusinessOperations.API.Validators.Authentication;
using Xunit;

namespace BusinessOperations.API.Tests.Authentication;

public sealed class LoginValidatorTests
{
    [Fact]
    public void Credentials_are_required()
    {
        var result = new LoginRequestValidator().Validate(new LoginRequest());
        Assert.Equal(2, result.Errors.Count);
    }

    [Fact]
    public void Optional_authenticator_code_does_not_change_password_validation()
    {
        var result = new LoginRequestValidator().Validate(new LoginRequest
        {
            UsernameOrEmail = "admin", Password = "Password1!", TwoFactorCode = "123456"
        });
        Assert.True(result.IsValid);
    }
}
