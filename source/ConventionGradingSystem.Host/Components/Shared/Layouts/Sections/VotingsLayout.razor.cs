using ConventionGradingSystem.Host.Components.Shared.Models;
using ConventionGradingSystem.Host.Configuration;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Layouts.Sections;

public partial class VotingsLayout
{
    private readonly IOptions<ApplicationConfiguration> _applicationConfiguration;
    private readonly VotingsLayoutModel _viewModel = new();

    public VotingsLayout(IOptions<ApplicationConfiguration> applicationConfiguration)
    {
        _applicationConfiguration = applicationConfiguration;
    }

    protected override void OnInitialized()
    {
        _viewModel.VotingsSectionAvailable = _applicationConfiguration.Value.VotingsFeatureEnabled;
    }
}
