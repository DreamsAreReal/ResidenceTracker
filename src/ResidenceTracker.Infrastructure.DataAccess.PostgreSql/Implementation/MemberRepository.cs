using Microsoft.Extensions.Logging;
using ResidenceTracker.Domain.Entities;
using ResidenceTracker.Infastructure.DataAccess.PostgreSql;
using ResidenceTracker.Infrastructure.DataAccess.PostgreSql.Abstractions;

namespace ResidenceTracker.Infrastructure.DataAccess.PostgreSql.Implementation;

public class MemberRepository
    (ApplicationDbContext context, ILogger<AbstractRepository<Member>> logger) : AbstractRepository<Member>(
        context, logger
    )
{
    protected override IQueryable<Member> BuildSearchQuery(IQueryable<Member> query, string searchText)
    {
        return query.Where(x => x.Name.Contains(searchText));
    }
}