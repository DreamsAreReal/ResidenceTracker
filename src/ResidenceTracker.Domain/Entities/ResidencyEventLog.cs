using ResidenceTracker.Domain.Abstractions;
using ResidenceTracker.Domain.Attributes;
using ResidenceTracker.Domain.Entities.Enums;

namespace ResidenceTracker.Domain.Entities;

public class ResidencyEventLog : AbstractEntity
{
    [HumanizedName("Тип события")]
    public ResidencyEventType EventType { get; set; }

    [HumanizedName("Квартира")]
    public Flat Flat { get; set; }

    [HumanizedName("Человек")]
    public Member Member { get; set; }

    // ReSharper disable once EmptyConstructor
    public ResidencyEventLog()
    {
    }
}