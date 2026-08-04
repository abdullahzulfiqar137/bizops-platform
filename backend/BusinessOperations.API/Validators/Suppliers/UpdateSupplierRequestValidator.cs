using BusinessOperations.API.DTOs.Suppliers;
using FluentValidation;

namespace BusinessOperations.API.Validators.Suppliers;

public sealed class UpdateSupplierRequestValidator : AbstractValidator<UpdateSupplierRequest>
{
    public UpdateSupplierRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Contact name is required.")
            .MaximumLength(150).WithMessage("Contact name must not exceed 150 characters.");

        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(150).WithMessage("Company name must not exceed 150 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(30).WithMessage("Phone number must not exceed 30 characters.");
    }
}
