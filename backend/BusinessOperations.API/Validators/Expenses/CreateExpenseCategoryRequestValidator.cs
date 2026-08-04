using BusinessOperations.API.DTOs.Expenses;
using FluentValidation;

namespace BusinessOperations.API.Validators.Expenses;

public sealed class CreateExpenseCategoryRequestValidator : AbstractValidator<CreateExpenseCategoryRequest>
{
    public CreateExpenseCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Expense category name is required.")
            .MaximumLength(100).WithMessage("Expense category name must not exceed 100 characters.");
    }
}
