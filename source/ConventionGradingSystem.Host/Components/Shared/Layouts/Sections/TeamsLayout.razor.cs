using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.Host.Components.Shared.Models;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Layouts.Sections;

public partial class TeamsLayout
{
    private readonly IOptions<DataConfiguration> _dataConfiguration;
    private readonly TeamsLayoutModel _viewModel = new();

    public TeamsLayout(IOptions<DataConfiguration> dataConfiguration)
    {
        _dataConfiguration = dataConfiguration;
    }

    protected override void OnInitialized()
    {
        _viewModel.TeamsSectionAvailable =
            _dataConfiguration.Value.Teams.Count > 0 &&
            _dataConfiguration.Value.Contests.Count > 0;
    }
}
