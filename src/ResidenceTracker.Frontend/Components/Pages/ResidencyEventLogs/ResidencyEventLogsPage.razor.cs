using Microsoft.AspNetCore.Components;
using ResidenceTracker.Domain.Entities;
using ResidenceTracker.Frontend.Components.Abstractions;
using ResidenceTracker.Infrastructure.Abstractions;

namespace ResidenceTracker.Frontend.Components.Pages.ResidencyEventLogs;

public partial class ResidencyEventLogsPage : AbstractCrudBasePage<ResidencyEventLog>
{
    protected override IReadOnlyCollection<string> DisplayOrder { get; } = new List<string>
    {
        "Id",
        "Flat.Number",
        "Member.Name",
        "CreatedAt",
        "ModifiedAt",
    };
    [Inject]
    protected IRepository<Flat> FlatRepository { get; set; } = null!;
}