using ResidenceTracker.Domain.Abstractions;

namespace ResidenceTracker.Domain.Entities;

public class House : AbstractEntity
{
    public required int Number { get; set; }
}