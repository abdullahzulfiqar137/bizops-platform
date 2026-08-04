using BusinessOperations.API.DTOs.Payments;
using BusinessOperations.API.Enums;
using FluentValidation;

namespace BusinessOperations.API.Validators.Payments;

public sealed class CreatePaymentRequestValidator : AbstractValidator<CreatePaymentRequest>
{
    public CreatePaymentRequestValidator()
    {
        RuleFor(x => x.PaymentType)
            .IsInEnum().WithMessage("Valid payment type is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Payment amount must be greater than zero.");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage("Valid payment method is required.");

        RuleFor(x => x.CustomerId)
            .NotEmpty().When(x => x.PaymentType == PaymentType.CustomerPayment)
            .WithMessage("Customer ID is required for Customer Payment.");

        RuleFor(x => x.SupplierId)
            .NotEmpty().When(x => x.PaymentType == PaymentType.SupplierPayment)
            .WithMessage("Supplier ID is required for Supplier Payment.");
    }
}
