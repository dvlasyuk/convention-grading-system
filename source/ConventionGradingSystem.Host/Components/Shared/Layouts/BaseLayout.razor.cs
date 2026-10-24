using ConventionGradingSystem.Host.Components.Shared.Models;
using ConventionGradingSystem.Host.Configuration;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Layouts;

public partial class BaseLayout
{
    private readonly IOptions<ApplicationConfiguration> _applicationConfiguration;
    private readonly BaseLayoutModel _viewModel = new();

    public BaseLayout(IOptions<ApplicationConfiguration> applicationConfiguration)
    {
        _applicationConfiguration = applicationConfiguration;
    }

    protected override void OnInitialized()
    {
        _viewModel.ConventionName = _applicationConfiguration.Value.ConventionName;
    }
}
