using ConventionGradingSystem.Host.Components.Shared.Models;
using ConventionGradingSystem.Host.Configuration;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Layouts.Sections;

public partial class TeamsLayout
{
    private readonly IOptions<ApplicationConfiguration> _applicationConfiguration;
    private readonly TeamsLayoutModel _viewModel = new();

    public TeamsLayout(IOptions<ApplicationConfiguration> applicationConfiguration)
    {
        _applicationConfiguration = applicationConfiguration;
    }

    protected override void OnInitialized()
    {
        _viewModel.TeamsSectionAvailable = _applicationConfiguration.Value.TeamsFeatureEnabled;
    }
}
