using FluentValidation;
using ResidenceTracker.Domain.Entities;
using ResidenceTracker.UseCases.Utilities;
using ResidenceTracker.UseCases.Validation.Abstractions;

namespace ResidenceTracker.UseCases.Validation.Implementation;

public class BillValidator : AbstractFormValidator<Bill>
{
    public BillValidator()
    {
        RuleFor(x => x.Flat)
            .NotEmpty()
            .WithMessage(
                $"{typeof(Bill).GetProperty(nameof(Bill.Flat))!.GetLocalizedDisplayName()} не может быть пустым"
            );

        RuleFor(x => x.AmountInRubles)
            .NotEmpty()
            .WithMessage(
                $"{typeof(Bill).GetProperty(nameof(Bill.AmountInRubles))!.GetLocalizedDisplayName()} не может быть пустым"
            )
            .GreaterThan(0)
            .WithMessage(
                $"{typeof(Bill).GetProperty(nameof(Bill.AmountInRubles))!.GetLocalizedDisplayName()} должен быть больше нуля"
            );
    }
}