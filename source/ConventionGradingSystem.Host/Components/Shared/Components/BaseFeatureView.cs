using ConventionGradingSystem.Host.Components.Shared.Models;
using ConventionGradingSystem.Host.Configuration;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Components;

public abstract class BaseFeatureView : ComponentBase
{
    private readonly IOptions<ApplicationConfiguration> _applicationConfiguration;

    [Parameter]
    public ApplicationFeature Feature { get; set; }

    protected bool IsFeatureEnabled { get; private set; }

    protected BaseFeatureView(IOptions<ApplicationConfiguration> applicationConfiguration)
    {
        _applicationConfiguration = applicationConfiguration;
    }

    protected override void OnInitialized()
    {
        IsFeatureEnabled = Feature switch
        {
            ApplicationFeature.Contests => _applicationConfiguration.Value.ContestsFeatureEnabled,
            ApplicationFeature.Votings => _applicationConfiguration.Value.VotingsFeatureEnabled,
            ApplicationFeature.Teams => _applicationConfiguration.Value.TeamsFeatureEnabled,
            ApplicationFeature.None => true,
            _ => true,
        };
    }
}
