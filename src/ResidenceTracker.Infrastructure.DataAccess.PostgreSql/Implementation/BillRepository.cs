using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResidenceTracker.Domain.Entities;
using ResidenceTracker.Infastructure.DataAccess.PostgreSql;
using ResidenceTracker.Infrastructure.DataAccess.PostgreSql.Abstractions;

namespace ResidenceTracker.Infrastructure.DataAccess.PostgreSql.Implementation;

public class BillRepository
    (ApplicationDbContext context, ILogger<AbstractRepository<Bill>> logger) : AbstractRepository<Bill>(
        context, logger
    )
{
    protected override IQueryable<Bill> BuildRelationShipQuery(IQueryable<Bill> query)
    {
        return query.Include(x => x.Flat);
    }

    protected override IQueryable<Bill> BuildSearchQuery(IQueryable<Bill> query, string searchText)
    {
        return query.Include(x => x.Flat).Where(x => x.Flat.Number.ToString().Contains(searchText));
    }
}