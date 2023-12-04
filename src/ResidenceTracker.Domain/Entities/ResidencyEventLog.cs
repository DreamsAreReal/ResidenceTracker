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
    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public ResidencyEventLog()
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }
}