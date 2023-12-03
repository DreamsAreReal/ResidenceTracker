using ResidenceTracker.Domain.Abstractions;

namespace ResidenceTracker.Domain.Entities;

public class ResidencyEventLog : AbstractEntity
{
    public required House House { get; set; }
    public required Flat Flat { get; set; }
    public required Human Human { get; set; }
    public required ResidencyEventLog EventLog { get; set; }
}