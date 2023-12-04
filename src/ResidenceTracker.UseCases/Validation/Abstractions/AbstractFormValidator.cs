using FluentValidation;
using FluentValidation.Results;

namespace ResidenceTracker.UseCases.Validation.Abstractions;

public abstract class AbstractFormValidator<T> : AbstractValidator<T>
{
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue =>
        async (model, propertyName) =>
        {
            ValidationResult? result = await ValidateAsync(
                                           ValidationContext<T>.CreateWithOptions(
                                               (T)model, x => x.IncludeProperties(propertyName)
                                           )
                                       );

            return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
        };
}