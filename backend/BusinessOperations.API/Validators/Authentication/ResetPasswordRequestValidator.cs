using BusinessOperations.API.DTOs.Authentication;
using FluentValidation;

namespace BusinessOperations.API.Validators.Authentication;

public sealed class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).MaximumLength(128);
        RuleFor(x => x.ConfirmPassword).Equal(x => x.NewPassword)
            .WithMessage("Password confirmation does not match.");
    }
}
