using FluentValidation;
using ResidenceTracker.Domain.Entities;
using ResidenceTracker.UseCases.Utilities;
using ResidenceTracker.UseCases.Validation.Abstractions;

namespace ResidenceTracker.UseCases.Validation.Implementation;

public class FlatValidator : AbstractFormValidator<Flat>
{
    public FlatValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty()
            .WithMessage(
                $"{typeof(Flat).GetProperty(nameof(Flat.Number))!.GetLocalizedDisplayName()} не может быть пустым"
            );
    }
}