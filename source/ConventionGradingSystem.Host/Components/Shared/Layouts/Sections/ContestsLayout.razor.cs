using ConventionGradingSystem.Host.Components.Shared.Models;
using ConventionGradingSystem.Host.Configuration;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Layouts.Sections;

public partial class ContestsLayout
{
    private readonly IOptions<ApplicationConfiguration> _applicationConfiguration;
    private readonly ContestsLayoutModel _viewModel = new();

    public ContestsLayout(IOptions<ApplicationConfiguration> applicationConfiguration)
    {
        _applicationConfiguration = applicationConfiguration;
    }

    protected override void OnInitialized()
    {
        _viewModel.ContestsSectionAvailable = _applicationConfiguration.Value.ContestsFeatureEnabled;
    }
}
