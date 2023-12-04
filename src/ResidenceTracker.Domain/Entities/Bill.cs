using ResidenceTracker.Domain.Abstractions;
using ResidenceTracker.Domain.Attributes;

namespace ResidenceTracker.Domain.Entities;

public class Bill : AbstractEntity
{
    [HumanizedName("Сумма руб.")]
    public decimal AmountInRubles { get; set; }

    [HumanizedName("Квартира")]
    public Flat Flat { get; set; }

    [HumanizedName("Оплачено в (UTC)")]
    public DateTime? PaidIn { get; set; }

    // ReSharper disable once EmptyConstructor
    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Bill()
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }
}