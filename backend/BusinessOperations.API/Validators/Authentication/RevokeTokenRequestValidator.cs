using BusinessOperations.API.DTOs.Authentication;
using FluentValidation;

namespace BusinessOperations.API.Validators.Authentication;

public sealed class RevokeTokenRequestValidator : AbstractValidator<RevokeTokenRequest>
{
    public RevokeTokenRequestValidator()
    {
    }
}
