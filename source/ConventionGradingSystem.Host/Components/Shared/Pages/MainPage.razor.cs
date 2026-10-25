using ConventionGradingSystem.Host.Components.Shared.Models;
using ConventionGradingSystem.Host.Configuration;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Pages;

public partial class MainPage
{
    private readonly IOptions<ApplicationConfiguration> _applicationConfiguration;
    private readonly MainPageModel _viewModel = new();

    public MainPage(IOptions<ApplicationConfiguration> applicationConfiguration)
    {
        _applicationConfiguration = applicationConfiguration;
    }

    protected override void OnInitialized()
    {
        _viewModel.ConventionName = _applicationConfiguration.Value.ConventionName;
    }
}
