using BusinessOperations.API.DTOs.Accounting;
using FluentValidation;

namespace BusinessOperations.API.Validators.Accounting;

public sealed class CreateJournalRequestValidator : AbstractValidator<CreateJournalRequest>
{
    public CreateJournalRequestValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Lines).NotNull().Must(x => x.Count >= 2)
            .WithMessage("A journal requires at least two lines.");
        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(x => x.AccountId).NotEmpty();
            line.RuleFor(x => x).Must(x => x.Debit >= 0 && x.Credit >= 0 && (x.Debit > 0) != (x.Credit > 0))
                .WithMessage("Each line must contain either a positive debit or a positive credit.");
            line.RuleFor(x => x.Memo).MaximumLength(300);
        });
        RuleFor(x => x.Lines).Must(lines =>
            lines is not null && Math.Abs(lines.Sum(x => x.Debit) - lines.Sum(x => x.Credit)) < 0.01m)
            .WithMessage("Total debits and credits must balance.");
    }
}
