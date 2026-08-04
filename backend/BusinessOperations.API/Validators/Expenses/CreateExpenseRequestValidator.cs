using BusinessOperations.API.DTOs.Expenses;
using FluentValidation;

namespace BusinessOperations.API.Validators.Expenses;

public sealed class CreateExpenseRequestValidator : AbstractValidator<CreateExpenseRequest>
{
    public CreateExpenseRequestValidator()
    {
        RuleFor(x => x.ExpenseCategoryId)
            .NotEmpty().WithMessage("Expense category ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Expense title is required.")
            .MaximumLength(150).WithMessage("Expense title must not exceed 150 characters.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Expense amount must be greater than zero.");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage("Valid payment method is required.");
    }
}
