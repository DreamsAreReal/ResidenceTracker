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
    public Bill()
    {
    }
}