using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.Host.Components.Shared.Models;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Layouts.Sections;

public partial class VotingsLayout
{
    private readonly IOptions<DataConfiguration> _dataConfiguration;
    private readonly VotingsLayoutModel _viewModel = new();

    public VotingsLayout(IOptions<DataConfiguration> dataConfiguration)
    {
        _dataConfiguration = dataConfiguration;
    }

    protected override void OnInitialized()
    {
        _viewModel.VotingsSectionAvailable = _dataConfiguration.Value.Votings.Count > 0;
    }
}
