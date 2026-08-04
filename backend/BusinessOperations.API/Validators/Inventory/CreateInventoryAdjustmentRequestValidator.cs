using BusinessOperations.API.DTOs.Inventory;
using FluentValidation;

namespace BusinessOperations.API.Validators.Inventory;

public sealed class CreateInventoryAdjustmentRequestValidator : AbstractValidator<CreateInventoryAdjustmentRequest>
{
    public CreateInventoryAdjustmentRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.AdjustmentType)
            .IsInEnum().WithMessage("Invalid adjustment type.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason for adjustment is required.")
            .MaximumLength(200).WithMessage("Reason must not exceed 200 characters.");
    }
}
