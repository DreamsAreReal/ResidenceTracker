namespace ResidenceTracker.Domain.Pagination;

public record PagedResult<T>(IReadOnlyCollection<T> Data, int TotalCount, int PageNumber, int TotalPages);