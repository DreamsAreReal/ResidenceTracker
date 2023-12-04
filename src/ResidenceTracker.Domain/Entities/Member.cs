using ResidenceTracker.Domain.Abstractions;
using ResidenceTracker.Domain.Attributes;

namespace ResidenceTracker.Domain.Entities;

public class Member : AbstractEntity
{
    public Guid? FlatId { get; }
    [HumanizedName("Имя")]
    public string Name { get; set; }

    // ReSharper disable once EmptyConstructor
    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Member()
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }
}