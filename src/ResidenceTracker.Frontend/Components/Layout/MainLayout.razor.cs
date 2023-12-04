using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace ResidenceTracker.Frontend.Components.Layout;

public partial class MainLayout : LayoutComponentBase
{
    [Inject]
    protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;
    private bool _drawerOpen;

    private void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }
}