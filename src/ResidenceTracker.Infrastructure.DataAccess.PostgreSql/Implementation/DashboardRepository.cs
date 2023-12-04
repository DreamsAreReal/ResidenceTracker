using System.Diagnostics;
using System.Runtime.Intrinsics.X86;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using ResidenceTracker.Domain.Entities;
using ResidenceTracker.Infastructure.DataAccess.PostgreSql;

namespace ResidenceTracker.Infrastructure.DataAccess.PostgreSql.Implementation;

public class DashboardRepository(ApplicationDbContext context, ILogger<DashboardRepository> logger)
{
    public async Task<IEnumerable<DashboardDataItem>> Get()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
       
        var dashboardDataItems = context.Houses.Include(x => x.Flats)
                                        .ThenInclude(x => x.Bills)
                                        .AsEnumerable()
                                        .Select(x => new DashboardDataItem
                                        {
                                            HouseNumber = x.Number,
                                            FlatsCount = x.Flats?.Count ?? 0,
                                            TotalPaidAmount = x.Flats?.Sum(f => f.Bills?.Sum(b => b.AmountInRubles) ?? 0) ?? 0
                                        })
                                        .ToList();


        stopwatch.Stop();
        logger.LogInformation("Get dashboard in {Time} ms", stopwatch.ElapsedMilliseconds);
        return dashboardDataItems;
    }
}