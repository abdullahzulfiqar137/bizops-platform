using BusinessOperations.API.DTOs.Sales;
using FluentValidation;

namespace BusinessOperations.API.Validators.Sales;

public sealed class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    public CreateSaleRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required.");

        RuleFor(x => x.TaxRate)
            .GreaterThanOrEqualTo(0).WithMessage("Tax rate cannot be negative.");

        RuleFor(x => x.DiscountAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Discount amount cannot be negative.");

        RuleFor(x => x.PaidAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Paid amount cannot be negative.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one sale item is required.");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateSaleItemRequestValidator());
    }
}
