using ResidenceTracker.Domain.Abstractions;

namespace ResidenceTracker.Domain.Entities;

public class Flat : AbstractEntity
{
    public required string Name { get; set; }
    public IEnumerable<Human> Members { get; set; } = new List<Human>();
}