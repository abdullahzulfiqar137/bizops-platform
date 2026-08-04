using BusinessOperations.API.DTOs.Businesses;
using FluentValidation;

namespace BusinessOperations.API.Validators.Businesses;

public sealed class UpdateBusinessProfileRequestValidator : AbstractValidator<UpdateBusinessProfileRequest>
{
    public UpdateBusinessProfileRequestValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(150).WithMessage("Company name must not exceed 150 characters.");

        RuleFor(x => x.LegalName)
            .NotEmpty().WithMessage("Legal name is required.")
            .MaximumLength(150).WithMessage("Legal name must not exceed 150 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(30).WithMessage("Phone number must not exceed 30 characters.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(250).WithMessage("Address must not exceed 250 characters.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required.");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Postal code is required.");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required.");

        RuleFor(x => x.CurrencyCode)
            .NotEmpty().WithMessage("Currency code is required.")
            .Length(3).WithMessage("Currency code must be 3 characters.");

        RuleFor(x => x.CurrencySymbol)
            .NotEmpty().WithMessage("Currency symbol is required.");

        RuleFor(x => x.TaxRate)
            .GreaterThanOrEqualTo(0).WithMessage("Tax rate cannot be negative.");

        RuleFor(x => x.InvoicePrefix)
            .NotEmpty().WithMessage("Invoice prefix is required.")
            .MaximumLength(20).WithMessage("Invoice prefix must not exceed 20 characters.");

        RuleFor(x => x.NextInvoiceNumber)
            .GreaterThan(0).WithMessage("Next invoice number must be greater than 0.");
    }
}
