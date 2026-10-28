using ConventionGradingSystem.Host.Components.Shared.Models;
using ConventionGradingSystem.Host.Configuration;
using ConventionGradingSystem.Host.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Layouts;

public partial class MainLayout
{
    public const string HeaderActionsSection = "HeaderActionsSection";

    private readonly AuthenticationProvider _authenticationProvider;
    private readonly IOptions<ApplicationConfiguration> _applicationConfiguration;
    private readonly NavigationManager _navigationManager;

    private MainLayoutModel? _viewModel;

    public MainLayout(
        AuthenticationProvider authenticationProvider,
        NavigationManager navigationManager,
        IOptions<ApplicationConfiguration> applicationConfiguration)
    {
        _authenticationProvider = authenticationProvider;
        _navigationManager = navigationManager;
        _applicationConfiguration = applicationConfiguration;
    }

    protected override void OnInitialized()
    {
        _viewModel = new(
            ConventionName: _applicationConfiguration.Value.ConventionName,
            IsDrawerOpen: false);
    }

    private void ToogleDrawer() => _viewModel!.IsDrawerOpen = !_viewModel!.IsDrawerOpen;

    private async Task SignOutFromSystem()
    {
        await _authenticationProvider.SignOut();
        _navigationManager.NavigateTo(_navigationManager.Uri, true);
    }
}
