using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.Host.Components.Shared.Models;
using ConventionGradingSystem.Host.Configuration;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Layouts;

public partial class MainLayout
{
    private readonly AuthenticationProvider _authenticationProvider;
    private readonly IOptions<ApplicationConfiguration> _applicationConfiguration;
    private readonly IOptions<DataConfiguration> _dataConfiguration;
    private readonly NavigationManager _navigationManager;

    private readonly MainLayoutModel _viewModel = new();

    public MainLayout(
        AuthenticationProvider authenticationProvider,
        IOptions<ApplicationConfiguration> applicationConfiguration,
        IOptions<DataConfiguration> dataConfiguration,
        NavigationManager navigationManager)
    {
        _authenticationProvider = authenticationProvider;
        _applicationConfiguration = applicationConfiguration;
        _dataConfiguration = dataConfiguration;
        _navigationManager = navigationManager;
    }

    protected override void OnInitialized()
    {
        _viewModel.ConventionName = _applicationConfiguration.Value.ConventionName;

        _viewModel.ContestsSectionAvailable = _dataConfiguration.Value.Contests.Count > 0;
        _viewModel.VotingsSectionAvailable = _dataConfiguration.Value.Votings.Count > 0;
        _viewModel.TeamsSectionAvailable =
            _dataConfiguration.Value.Teams.Count > 0 &&
            _dataConfiguration.Value.Contests.Count > 0;
    }

    private void ToogleDrawer() => _viewModel.IsDrawerOpen = !_viewModel.IsDrawerOpen;

    private async Task OnSignOutFromSystem()
    {
        await _authenticationProvider.SignOut();
        _navigationManager.NavigateTo(_navigationManager.Uri, true);
    }
}
