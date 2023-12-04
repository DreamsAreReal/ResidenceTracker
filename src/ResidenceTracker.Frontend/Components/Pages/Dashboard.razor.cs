using Microsoft.AspNetCore.Components;
using ResidenceTracker.Domain.Entities;
using ResidenceTracker.Infrastructure.DataAccess.PostgreSql.Implementation;

namespace ResidenceTracker.Frontend.Components.Pages;

public partial class Dashboard : ComponentBase
{
    [Inject]
    protected DashboardRepository DashboardRepository { get; set; } = null!;
    private IEnumerable<DashboardDataItem> Data = new List<DashboardDataItem>();

    protected override async Task OnInitializedAsync()
    {
        Data = await DashboardRepository.Get();
    }
}