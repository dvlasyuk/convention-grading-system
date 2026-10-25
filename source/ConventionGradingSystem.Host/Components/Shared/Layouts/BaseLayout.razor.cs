using ConventionGradingSystem.Host.Components.Shared.Models;
using ConventionGradingSystem.Host.Configuration;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Layouts;

public partial class BaseLayout
{
    private readonly IOptions<ApplicationConfiguration> _applicationConfiguration;
    private BaseLayoutModel? _viewModel;

    public BaseLayout(IOptions<ApplicationConfiguration> applicationConfiguration)
    {
        _applicationConfiguration = applicationConfiguration;
    }

    protected override void OnInitialized()
    {
        _viewModel = new(_applicationConfiguration.Value.ConventionName);
    }
}
