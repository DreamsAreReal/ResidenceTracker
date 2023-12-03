using ResidenceTracker.Domain.Abstractions;

namespace ResidenceTracker.Domain.Entities;

public class Bill : AbstractEntity
{
    public required Flat Flat { get; set; }
    public required decimal AmountInRubles { get; set; }
    public required DateTime PaidIn { get; set; }
}