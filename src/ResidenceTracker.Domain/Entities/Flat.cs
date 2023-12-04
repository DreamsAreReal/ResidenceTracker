using ResidenceTracker.Domain.Abstractions;
using ResidenceTracker.Domain.Attributes;

namespace ResidenceTracker.Domain.Entities;

public class Flat : AbstractEntity
{
    [HumanizedName("Жильцы")]
    public ICollection<Member>? Members { get; set; } = new List<Member>();

    [HumanizedName("Номер")]
    public int Number { get; set; }

    // ReSharper disable once EmptyConstructor
    public Flat()
    {
    }
}