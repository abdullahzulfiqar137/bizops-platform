using BusinessOperations.API.DTOs.Products;
using FluentValidation;

namespace BusinessOperations.API.Validators.Products;

public sealed class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(150).WithMessage("Product name must not exceed 150 characters.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required.");

        RuleFor(x => x.UnitId)
            .NotEmpty().WithMessage("Product unit is required.");

        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Cost price cannot be negative.");

        RuleFor(x => x.SalePrice)
            .GreaterThanOrEqualTo(0).WithMessage("Sale price cannot be negative.");

        RuleFor(x => x.InitialStockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Initial stock quantity cannot be negative.");

        RuleFor(x => x.MinimumStockLevel)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum stock level cannot be negative.");
    }
}
