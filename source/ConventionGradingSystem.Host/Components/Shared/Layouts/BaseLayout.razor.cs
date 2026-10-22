using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.Host.Components.Shared.Models;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Layouts;

public partial class BaseLayout
{
    private readonly IOptions<ApplicationConfiguration> _configuration;
    private readonly BaseLayoutModel _viewModel = new();

    public BaseLayout(IOptions<ApplicationConfiguration> configuration)
    {
        _configuration = configuration;
    }

    protected override void OnInitialized()
    {
        _viewModel.ConventionName = _configuration.Value.ConventionName;
    }
}
