using ResidenceTracker.Domain.Abstractions;
using ResidenceTracker.Domain.Attributes;

namespace ResidenceTracker.Domain.Entities;

public class House : AbstractEntity
{
    [HumanizedName("Квартиры")]
    public ICollection<Flat>? Flats { get; set; }

    [HumanizedName("Номер")]
    public int Number { get; set; }

    // ReSharper disable once EmptyConstructor
    public House()
    {
    }
}