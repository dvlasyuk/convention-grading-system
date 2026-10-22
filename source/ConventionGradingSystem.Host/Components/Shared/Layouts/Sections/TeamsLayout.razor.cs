using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.Host.Components.Shared.Models;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Layouts.Sections;

public partial class TeamsLayout
{
    private readonly IOptions<ApplicationConfiguration> _configuration;
    private readonly TeamsLayoutModel _viewModel = new();

    public TeamsLayout(IOptions<ApplicationConfiguration> configuration)
    {
        _configuration = configuration;
    }

    protected override void OnInitialized()
    {
        _viewModel.TeamsSectionAvailable =
            _configuration.Value.Teams.Count > 0 &&
            _configuration.Value.Contests.Count > 0;
    }
}
