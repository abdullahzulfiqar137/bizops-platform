using BusinessOperations.API.DTOs.Sales;
using FluentValidation;

namespace BusinessOperations.API.Validators.Sales;

public sealed class CreateSaleItemRequestValidator : AbstractValidator<CreateSaleItemRequest>
{
    public CreateSaleItemRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Item quantity must be greater than zero.");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative.");
    }
}
