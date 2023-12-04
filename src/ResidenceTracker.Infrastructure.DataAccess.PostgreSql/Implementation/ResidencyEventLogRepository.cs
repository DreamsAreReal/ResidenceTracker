using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResidenceTracker.Domain.Entities;
using ResidenceTracker.Infastructure.DataAccess.PostgreSql;
using ResidenceTracker.Infrastructure.DataAccess.PostgreSql.Abstractions;

namespace ResidenceTracker.Infrastructure.DataAccess.PostgreSql.Implementation;

public class ResidencyEventLogRepository(ApplicationDbContext context,
                                         ILogger<AbstractRepository<ResidencyEventLog>> logger
) : AbstractRepository<ResidencyEventLog>(context, logger)
{
    protected override IQueryable<ResidencyEventLog> BuildSearchQuery(
        IQueryable<ResidencyEventLog> query,
        string searchText
    )
    {
        return query.Include(x => x.Flat)
                    .Include(x => x.Member)
                    .Where(x => x.Flat.Number.ToString().Contains(searchText))
                    .Where(x=>x.Member.Name.Contains(searchText));
    }

    protected override IQueryable<ResidencyEventLog> BuildRelationShipQuery(IQueryable<ResidencyEventLog> query)
    {
        return query.Include(x => x.Member).Include(x => x.Flat);
    }
}