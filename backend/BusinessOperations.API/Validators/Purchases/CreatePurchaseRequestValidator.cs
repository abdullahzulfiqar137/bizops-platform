using BusinessOperations.API.DTOs.Purchases;
using FluentValidation;

namespace BusinessOperations.API.Validators.Purchases;

public sealed class CreatePurchaseRequestValidator : AbstractValidator<CreatePurchaseRequest>
{
    public CreatePurchaseRequestValidator()
    {
        RuleFor(x => x.SupplierId)
            .NotEmpty().WithMessage("Supplier ID is required.");

        RuleFor(x => x.TaxRate)
            .GreaterThanOrEqualTo(0).WithMessage("Tax rate cannot be negative.");

        RuleFor(x => x.DiscountAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Discount amount cannot be negative.");

        RuleFor(x => x.PaidAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Paid amount cannot be negative.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one purchase item is required.");

        RuleForEach(x => x.Items)
            .SetValidator(new CreatePurchaseItemRequestValidator());
    }
}
