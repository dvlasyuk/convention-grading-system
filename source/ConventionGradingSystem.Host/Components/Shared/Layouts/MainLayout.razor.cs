using ConventionGradingSystem.DataAccess.Configuration;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Layouts;

public partial class MainLayout
{
    private readonly AuthenticationProvider _authenticationProvider;
    private readonly IOptions<ApplicationConfiguration> _configuration;
    private readonly NavigationManager _navigationManager;

    private string _conventionName = string.Empty;

    private bool _contestsSectionAvailable;
    private bool _votingsSectionAvailable;
    private bool _teamsSectionAvailable;

    private bool _isDrawerOpen;

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
        _conventionName = _configuration.Value.ConventionName;

        _contestsSectionAvailable = _configuration.Value.Contests.Count > 0;
        _votingsSectionAvailable = _configuration.Value.Votings.Count > 0;
        _teamsSectionAvailable =
            _configuration.Value.Teams.Count > 0 &&
            _configuration.Value.Contests.Count > 0;
    }

    private void ToogleDrawer() => _isDrawerOpen = !_isDrawerOpen;

    private async Task OnSignOutFromSystem()
    {
        await _authenticationProvider.SignOut();
        _navigationManager.NavigateTo(_navigationManager.Uri, true);
    }
}
