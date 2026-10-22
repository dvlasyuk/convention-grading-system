using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.Host.Components.Shared.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Layouts;

public partial class MainLayout
{
    private readonly AuthenticationProvider _authenticationProvider;
    private readonly IOptions<ApplicationConfiguration> _configuration;
    private readonly NavigationManager _navigationManager;

    private readonly MainLayoutModel _viewModel = new();

    public MainLayout(
        AuthenticationProvider authenticationProvider,
        IOptions<ApplicationConfiguration> configuration,
        NavigationManager navigationManager)
    {
        _authenticationProvider = authenticationProvider;
        _configuration = configuration;
        _navigationManager = navigationManager;
    }

    protected override void OnInitialized()
    {
        _viewModel.ConventionName = _configuration.Value.ConventionName;

        _viewModel.ContestsSectionAvailable = _configuration.Value.Contests.Count > 0;
        _viewModel.VotingsSectionAvailable = _configuration.Value.Votings.Count > 0;
        _viewModel.TeamsSectionAvailable =
            _configuration.Value.Teams.Count > 0 &&
            _configuration.Value.Contests.Count > 0;
    }

    private void ToogleDrawer() => _viewModel.IsDrawerOpen = !_viewModel.IsDrawerOpen;

    private async Task OnSignOutFromSystem()
    {
        await _authenticationProvider.SignOut();
        _navigationManager.NavigateTo(_navigationManager.Uri, true);
    }
}
