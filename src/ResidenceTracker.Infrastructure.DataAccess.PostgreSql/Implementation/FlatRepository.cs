using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResidenceTracker.Domain.Entities;
using ResidenceTracker.Infastructure.DataAccess.PostgreSql;
using ResidenceTracker.Infrastructure.DataAccess.PostgreSql.Abstractions;

namespace ResidenceTracker.Infrastructure.DataAccess.PostgreSql.Implementation;

public class FlatRepository
    (ApplicationDbContext context, ILogger<AbstractRepository<Flat>> logger) : AbstractRepository<Flat>(
        context, logger
    )
{
    public override Task AddAsync(Flat entity, CancellationToken cancellationToken)
    {
        entity = RemoveEmptyResultsHistory(entity);
        return base.AddAsync(entity, cancellationToken);
    }

    public async Task<bool> HasMember(Guid flatId, Guid memberId, CancellationToken cancellationToken)
    {
        bool exists = await context.Flats.Include(x => x.Members)
                                   .Where(x => x.Id == flatId)
                                   .AnyAsync(
                                       x => x.Members != null && x.Members.Any(f => f.Id == memberId),
                                       cancellationToken
                                   );

        return exists;
    }

    public override Task UpdateAsync(Flat entity, CancellationToken cancellationToken)
    {
        entity = RemoveEmptyResultsHistory(entity);
        return base.UpdateAsync(entity, cancellationToken);
    }

    protected override IQueryable<Flat> BuildRelationShipQuery(IQueryable<Flat> query)
    {
        return query.Include(x => x.Members);
    }

    protected override IQueryable<Flat> BuildSearchQuery(IQueryable<Flat> query, string searchText)
    {
        return query.Where(x => x.Number.ToString().Contains(searchText));
    }

    private Flat RemoveEmptyResultsHistory(Flat entity)
    {
        if (entity.Members is not null)
            if (entity.Members.Count == 0)
                entity.Members = null;

        return entity;
    }
}