using ConventionGradingSystem.DataAccess.Configuration;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Layouts.Sections;

public partial class VotingsLayout
{
    private readonly IOptions<ApplicationConfiguration> _configuration;

    private bool _votingsSectionAvailable;

    public VotingsLayout(IOptions<ApplicationConfiguration> configuration)
    {
        _configuration = configuration;
    }

    protected override void OnInitialized()
    {
        _votingsSectionAvailable = _configuration.Value.Votings.Count > 0;
    }
}
