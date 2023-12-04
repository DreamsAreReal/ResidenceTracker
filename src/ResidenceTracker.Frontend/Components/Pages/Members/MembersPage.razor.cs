using ResidenceTracker.Domain.Entities;
using ResidenceTracker.Frontend.Components.Abstractions;

namespace ResidenceTracker.Frontend.Components.Pages.Members;

public partial class MembersPage : AbstractCrudBasePage<Member>
{
    protected override IReadOnlyCollection<string> DisplayOrder { get; } = new List<string>
    {
        "Id",
        "Name",
        "CreatedAt",
        "ModifiedAt",
    };
}