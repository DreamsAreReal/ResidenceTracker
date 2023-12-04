using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResidenceTracker.Domain.Abstractions;
using ResidenceTracker.Domain.Pagination;
using ResidenceTracker.Infrastructure.Abstractions;

namespace ResidenceTracker.Infrastructure.DataAccess.PostgreSql.Abstractions;

public abstract class AbstractRepository<T>(ApplicationDbContext context,
                                            ILogger<AbstractRepository<T>> logger
) : IRepository<T> where T : AbstractEntity
{
    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        await context.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        stopwatch.Stop();

        logger.LogInformation(
            "Added of type {Entity} with id {EntityId} in {Time} ms", entity.GetType().Name, entity.Id,
            stopwatch.ElapsedMilliseconds
        );
    }

    public virtual async Task DeleteBatchAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        context.RemoveRange(entities);
        await context.SaveChangesAsync(cancellationToken);
        stopwatch.Stop();
        IEnumerable<string> entityIds = entities.Select(e => e.Id.ToString());
        string idsString = string.Join(", ", entityIds);

        logger.LogInformation(
            "Removed collection of type {Entity} with id {EntityId} in {Time} ms", typeof(T).Name, idsString,
            stopwatch.ElapsedMilliseconds
        );
    }

    public virtual async Task<PagedResult<T>> GetPagedAsync(int pageNumber,
                                                            int pageSize,
                                                            CancellationToken cancellationToken
    )
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int startIndex = (pageNumber - 1) * pageSize;
        int totalCount = await context.Set<T>().CountAsync(cancellationToken);
        int totalPages = (int)Math.Ceiling(totalCount / (decimal)pageSize);

        if (pageNumber < 1)
            pageNumber = 1;

        if (pageNumber > totalPages)
            pageNumber = totalPages;

        IQueryable<T> query = context.Set<T>()
                                     .OrderByDescending(x => x.CreatedAt)
                                     .Skip(startIndex)
                                     .Take(pageSize)
                                     .AsQueryable();

        query = BuildRelationShipQuery(query);
        List<T> pagedEntities = await query.ToListAsync(cancellationToken);
        stopwatch.Stop();

        logger.LogInformation(
            "Retrieved page {PageNumber} of size {PageSize} for {EntityType} in {Time} ms", pageNumber,
            pageSize, typeof(T).Name, stopwatch.ElapsedMilliseconds
        );

        return new(pagedEntities, totalCount, pageNumber, totalPages);
    }

    public virtual async Task<PagedResult<T>> Search(string search,
                                                     int pageNumber,
                                                     int pageSize,
                                                     CancellationToken cancellationToken
    )
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int startIndex = (pageNumber - 1) * pageSize;
        IQueryable<T> query = context.Set<T>();

        if (!string.IsNullOrEmpty(search))
            query = BuildSearchQuery(query, search);

        int totalCount = query.Count();
        int actualTotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        pageNumber = Math.Min(pageNumber, actualTotalPages);
        startIndex = (pageNumber - 1) * pageSize;
        startIndex = Math.Max(startIndex, 0);

        query = query.OrderByDescending(x => EF.Property<DateTime>(x, "CreatedAt"))
                     .Skip(startIndex)
                     .Take(pageSize)
                     .AsQueryable();

        query = BuildRelationShipQuery(query);
        List<T> pagedEntities = await query.ToListAsync(cancellationToken);
        stopwatch.Stop();

        logger.LogInformation(
            "Retrieved searchable page {PageNumber} of size {PageSize} for {EntityType} with search term '{SearchTerm}' in {Time} ms",
            pageNumber, pageSize, typeof(T).Name, search, stopwatch.ElapsedMilliseconds
        );

        return new(pagedEntities, totalCount, pageNumber, actualTotalPages);
    }

    public async Task<IEnumerable<T>> SearchForAutocomplete(string value, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<T>();

        PagedResult<T> pagedResult = await Search(value, 1, 5, cancellationToken);
        return pagedResult.Data;
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        context.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        stopwatch.Stop();

        logger.LogInformation(
            "Updated of type {Entity} with id {EntityId} in {Time} ms", entity.GetType().Name, entity.Id,
            stopwatch.ElapsedMilliseconds
        );
    }

    public virtual async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        context.UpdateRange(entities);
        await context.SaveChangesAsync(cancellationToken);
        stopwatch.Stop();
        IEnumerable<string> entityIds = entities.Select(e => e.Id.ToString());
        string idsString = string.Join(", ", entityIds);

        logger.LogInformation(
            "Updated of type {Entity} with id {EntityId} in {Time} ms", typeof(T).Name, idsString,
            stopwatch.ElapsedMilliseconds
        );
    }

    protected virtual IQueryable<T> BuildRelationShipQuery(IQueryable<T> query)
    {
        return query;
    }

    protected virtual IQueryable<T> BuildSearchQuery(IQueryable<T> query, string searchText)
    {
        return query;
    }
}