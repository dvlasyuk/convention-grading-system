using ConventionGradingSystem.Host.Components.Shared.Models;
using ConventionGradingSystem.Host.Configuration;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Layouts;

public partial class MainLayout
{
    private readonly AuthenticationProvider _authenticationProvider;
    private readonly IOptions<ApplicationConfiguration> _applicationConfiguration;
    private readonly NavigationManager _navigationManager;

    private readonly MainLayoutModel _viewModel = new();

    public MainLayout(
        AuthenticationProvider authenticationProvider,
        IOptions<ApplicationConfiguration> applicationConfiguration,
        NavigationManager navigationManager)
    {
        _authenticationProvider = authenticationProvider;
        _applicationConfiguration = applicationConfiguration;
        _navigationManager = navigationManager;
    }

    protected override void OnInitialized()
    {
        _viewModel.ConventionName = _applicationConfiguration.Value.ConventionName;
    }

    private void ToogleDrawer() => _viewModel.IsDrawerOpen = !_viewModel.IsDrawerOpen;

    private async Task OnSignOutFromSystem()
    {
        await _authenticationProvider.SignOut();
        _navigationManager.NavigateTo(_navigationManager.Uri, true);
    }
}
