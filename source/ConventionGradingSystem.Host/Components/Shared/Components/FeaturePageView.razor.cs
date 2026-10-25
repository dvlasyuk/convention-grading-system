using ConventionGradingSystem.Host.Configuration;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Components;

public partial class FeaturePageView
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    public FeaturePageView(IOptions<ApplicationConfiguration> applicationConfiguration)
        : base(applicationConfiguration)
    { }
}
