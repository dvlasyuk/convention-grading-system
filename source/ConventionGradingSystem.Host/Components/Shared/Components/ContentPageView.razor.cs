using ConventionGradingSystem.Host.Components.Shared.Models;

using Microsoft.AspNetCore.Components;

namespace ConventionGradingSystem.Host.Components.Shared.Components;

public partial class ContentPageView
{
    [Parameter]
    public LoadingStatus LoadingStatus { get; set; }

    [Parameter]
    public LoadingError? LoadingError { get; set; }

    [Parameter]
    public ContentType ContentType { get; set; }

    [Parameter]
    public string? DefaultPageTitle { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

}
