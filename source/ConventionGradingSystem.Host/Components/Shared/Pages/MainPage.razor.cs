using ConventionGradingSystem.DataAccess.Configuration;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Pages;

public partial class MainPage
{
    private readonly IOptions<ApplicationConfiguration> _configuration;

    private string _conventionName = string.Empty;

    private bool _contestsSectionAvailable;
    private bool _votingsSectionAvailable;
    private bool _teamsSectionAvailable;

    public MainPage(IOptions<ApplicationConfiguration> configuration)
    {
        _configuration = configuration;
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
}
