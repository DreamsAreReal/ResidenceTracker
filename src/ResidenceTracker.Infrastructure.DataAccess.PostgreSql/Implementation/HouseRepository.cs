using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResidenceTracker.Domain.Entities;
using ResidenceTracker.Infastructure.DataAccess.PostgreSql;
using ResidenceTracker.Infrastructure.DataAccess.PostgreSql.Abstractions;

namespace ResidenceTracker.Infrastructure.DataAccess.PostgreSql.Implementation;

public class HouseRepository
    (ApplicationDbContext context, ILogger<AbstractRepository<House>> logger) : AbstractRepository<House>(
        context, logger
    )
{
    public override Task AddAsync(House entity, CancellationToken cancellationToken)
    {
        entity = RemoveEmptyResultsHistory(entity);
        return base.AddAsync(entity, cancellationToken);
    }

    public async Task<bool> HasFlat(Guid houseId, Guid flatId, CancellationToken cancellationToken)
    {
        bool exists = await context.Houses.Include(x => x.Flats)
                                   .Where(x => x.Id == houseId)
                                   .AnyAsync(
                                       x => x.Flats != null && x.Flats.Any(f => f.Id == flatId),
                                       cancellationToken
                                   );

        return exists;
    }

    public override Task UpdateAsync(House entity, CancellationToken cancellationToken)
    {
        entity = RemoveEmptyResultsHistory(entity);
        return base.UpdateAsync(entity, cancellationToken);
    }

    protected override IQueryable<House> BuildRelationShipQuery(IQueryable<House> query)
    {
        return query.Include(x => x.Flats);
    }

    protected override IQueryable<House> BuildSearchQuery(IQueryable<House> query, string searchText)
    {
        return query.Where(x => x.Number.ToString().Contains(searchText));
    }

    private House RemoveEmptyResultsHistory(House entity)
    {
        if (entity.Flats is not null)
            if (entity.Flats.Count == 0)
                entity.Flats = null;

        return entity;
    }
}