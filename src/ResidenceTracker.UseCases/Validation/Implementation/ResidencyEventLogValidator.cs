using FluentValidation;
using ResidenceTracker.Domain.Entities;
using ResidenceTracker.UseCases.Utilities;
using ResidenceTracker.UseCases.Validation.Abstractions;

namespace ResidenceTracker.UseCases.Validation.Implementation;

public class ResidencyEventLogValidator : AbstractFormValidator<ResidencyEventLog>
{
    public ResidencyEventLogValidator()
    {
        RuleFor(x => x.Flat)
            .NotEmpty()
            .WithMessage(
                $"{typeof(Bill).GetProperty(nameof(Bill.Flat))!.GetLocalizedDisplayName()} не может быть пустым"
            );
    }
}