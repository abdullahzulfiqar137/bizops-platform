using BusinessOperations.API.DTOs.Authentication;
using FluentValidation;

namespace BusinessOperations.API.Validators.Authentication;

public sealed class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Access token is required.");

    }
}
