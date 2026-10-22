using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.Host.Components.Shared.Models;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Layouts.Sections;

public partial class VotingsLayout
{
    private readonly IOptions<ApplicationConfiguration> _configuration;
    private readonly VotingsLayoutModel _viewModel = new();

    public VotingsLayout(IOptions<ApplicationConfiguration> configuration)
    {
        _configuration = configuration;
    }

    protected override void OnInitialized()
    {
        _viewModel.VotingsSectionAvailable = _configuration.Value.Votings.Count > 0;
    }
}
