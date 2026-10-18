using ConventionGradingSystem.DataAccess.Configuration;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Layouts.Sections;

public partial class TeamsLayout
{
    private readonly IOptions<ApplicationConfiguration> _configuration;

    private bool _teamsSectionAvailable;

    public TeamsLayout(IOptions<ApplicationConfiguration> configuration)
    {
        _configuration = configuration;
    }

    protected override void OnInitialized()
    {
        _teamsSectionAvailable =
            _configuration.Value.Teams.Count > 0 &&
            _configuration.Value.Contests.Count > 0;
    }
}
