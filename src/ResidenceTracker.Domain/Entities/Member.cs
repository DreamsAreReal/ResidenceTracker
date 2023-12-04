using ResidenceTracker.Domain.Abstractions;
using ResidenceTracker.Domain.Attributes;

namespace ResidenceTracker.Domain.Entities;

public class Member : AbstractEntity
{
    [HumanizedName("Имя")]
    public string Name { get; set; }

    // ReSharper disable once EmptyConstructor
    public Member()
    {
    }
}