using FluentValidation;
using ResidenceTracker.Domain.Entities;
using ResidenceTracker.UseCases.Utilities;
using ResidenceTracker.UseCases.Validation.Abstractions;

namespace ResidenceTracker.UseCases.Validation.Implementation;

public class HouseValidator : AbstractFormValidator<House>
{
    public HouseValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty()
            .WithMessage(
                $"{typeof(House).GetProperty(nameof(House.Number))!.GetLocalizedDisplayName()} не может быть пустым"
            );
    }
}