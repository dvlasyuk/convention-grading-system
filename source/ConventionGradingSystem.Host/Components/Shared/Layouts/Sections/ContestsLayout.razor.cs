using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.Host.Components.Shared.Models;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Layouts.Sections;

public partial class ContestsLayout
{
    private readonly IOptions<DataConfiguration> _dataConfiguration;
    private readonly ContestsLayoutModel _viewModel = new();

    public ContestsLayout(IOptions<DataConfiguration> dataConfiguration)
    {
        _dataConfiguration = dataConfiguration;
    }

    protected override void OnInitialized()
    {
        _viewModel.ContestsSectionAvailable = _dataConfiguration.Value.Contests.Count > 0;
    }
}
