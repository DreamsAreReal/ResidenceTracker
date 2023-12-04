namespace ResidenceTracker.Domain.Entities;

public record DashboardDataItem
{
    public int FlatsCount { get; set; }
    public int HouseNumber { get; set; }
    public decimal TotalPaidAmount { get; set; }
}