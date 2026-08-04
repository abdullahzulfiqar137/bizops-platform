using BusinessOperations.API.DTOs.Purchases;
using FluentValidation;

namespace BusinessOperations.API.Validators.Purchases;

public sealed class CreatePurchaseItemRequestValidator : AbstractValidator<CreatePurchaseItemRequest>
{
    public CreatePurchaseItemRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Item quantity must be greater than zero.");

        RuleFor(x => x.UnitCost)
            .GreaterThanOrEqualTo(0).WithMessage("Unit cost cannot be negative.");
    }
}
