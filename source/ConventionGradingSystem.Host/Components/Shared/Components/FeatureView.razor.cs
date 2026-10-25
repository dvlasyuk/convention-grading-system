using ConventionGradingSystem.Host.Configuration;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Components;

public partial class FeatureView
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public RenderFragment? Enabled { get; set; }

    [Parameter]
    public RenderFragment? Disabled { get; set; }

    public FeatureView(IOptions<ApplicationConfiguration> applicationConfiguration)
        : base(applicationConfiguration)
    { }
}
