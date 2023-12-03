using ResidenceTracker.Domain.Abstractions;

namespace ResidenceTracker.Domain.Entities;

public class Human : AbstractEntity
{
    public required string Name { get; set; }
}