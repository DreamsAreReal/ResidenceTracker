using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResidenceTracker.Domain.Entities;

namespace ResidenceTracker.Infrastructure.DataAccess.PostgreSql.Implementation;

public class DashboardRepository(ApplicationDbContext context, ILogger<DashboardRepository> logger)
{
    public Task<IEnumerable<DashboardDataItem>> Get()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        List<DashboardDataItem> dashboardDataItems = context.Houses.Include(x => x.Flats)!
                                                            .ThenInclude(x => x.Bills)
                                                            .AsEnumerable()
                                                            .Select(
                                                                x => new DashboardDataItem
                                                                {
                                                                    HouseNumber = x.Number,
                                                                    FlatsCount = x.Flats?.Count ?? 0,
                                                                    TotalPaidAmount =
                                                                        x.Flats?.Sum(
                                                                            f => f.Bills?.Sum(
                                                                                    b => b.AmountInRubles
                                                                                ) ??
                                                                                0
                                                                        ) ??
                                                                        0,
                                                                }
                                                            )
                                                            .ToList();

        stopwatch.Stop();
        logger.LogInformation("Get dashboard in {Time} ms", stopwatch.ElapsedMilliseconds);
        return Task.FromResult<IEnumerable<DashboardDataItem>>(dashboardDataItems);
    }
}