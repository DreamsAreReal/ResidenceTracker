using Microsoft.AspNetCore.Components;
using ResidenceTracker.Domain.Entities;
using ResidenceTracker.Infrastructure.DataAccess.PostgreSql.Implementation;

namespace ResidenceTracker.Frontend.Components.Pages;

public  partial class Dashboard : ComponentBase
{
    private IEnumerable<DashboardDataItem> Data = new List<DashboardDataItem>();

    [Inject]
    protected DashboardRepository DashboardRepository { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Data = await DashboardRepository.Get();
        return;
    }
}