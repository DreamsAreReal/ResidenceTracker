using ResidenceTracker.Domain.Abstractions;
using ResidenceTracker.Domain.Pagination;

namespace ResidenceTracker.Infrastructure.Abstractions;

public interface IRepository<T> where T : AbstractEntity
{
    Task AddAsync(T entity, CancellationToken cancellationToken);
    Task DeleteBatchAsync(IEnumerable<T> entities, CancellationToken cancellationToken);
    Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<PagedResult<T>> Search(string search,
                                int pageNumber,
                                int pageSize,
                                CancellationToken cancellationToken
    );

    Task<IEnumerable<T>> SearchForAutocomplete(string value, CancellationToken cancellationToken);

    Task UpdateAsync(T entity, CancellationToken cancellationToken);

    Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken);
}