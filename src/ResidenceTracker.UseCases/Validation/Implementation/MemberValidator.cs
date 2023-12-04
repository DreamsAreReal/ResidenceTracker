using FluentValidation;
using ResidenceTracker.Domain.Entities;
using ResidenceTracker.UseCases.Utilities;
using ResidenceTracker.UseCases.Validation.Abstractions;

namespace ResidenceTracker.UseCases.Validation.Implementation;

public class MemberValidator : AbstractFormValidator<Member>
{
    public MemberValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(
                $"{typeof(Member).GetProperty(nameof(Member.Name))!.GetLocalizedDisplayName()} не может быть пустым"
            );
    }
}