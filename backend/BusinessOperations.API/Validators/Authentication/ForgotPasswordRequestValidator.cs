using BusinessOperations.API.DTOs.Authentication;
using FluentValidation;

namespace BusinessOperations.API.Validators.Authentication;

public sealed class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
    }
}
