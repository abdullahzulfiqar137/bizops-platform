using BusinessOperations.API.DTOs.Sales;
using FluentValidation;

namespace BusinessOperations.API.Validators.Returns;

public sealed class CreateSaleReturnRequestValidator : AbstractValidator<CreateSaleReturnRequest>
{
    public CreateSaleReturnRequestValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Notes).MaximumLength(500);
        RuleFor(x => x.Items).NotEmpty().Must(items => items.Select(x => x.SaleItemId).Distinct().Count() == items.Count)
            .WithMessage("The same item cannot appear more than once.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.SaleItemId).NotEmpty();
            item.RuleFor(x => x.Quantity).GreaterThan(0);
        });
    }
}
