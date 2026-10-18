using ConventionGradingSystem.DataAccess.Configuration;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Layouts.Sections;

public partial class ContestsLayout
{
    private readonly IOptions<ApplicationConfiguration> _configuration;

    public ContestsLayout(IOptions<ApplicationConfiguration> configuration)
    {
        _configuration = configuration;
    }

    private bool _contestsSectionAvailable;

    protected override void OnInitialized()
    {
        _contestsSectionAvailable = _configuration.Value.Contests.Count > 0;
    }
}
