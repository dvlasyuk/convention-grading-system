using ConventionGradingSystem.Host.Components.Shared.Models;
using ConventionGradingSystem.Host.Configuration;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Pages;

public partial class MainPage
{
    private readonly IOptions<ApplicationConfiguration> _applicationConfiguration;

    public MainPage(IOptions<ApplicationConfiguration> applicationConfiguration)
    {
        _applicationConfiguration = applicationConfiguration;
    }

    protected override Task<ContentState<MainPageModel>> LoadContent()
    {
        var viewModel = new MainPageModel(_applicationConfiguration.Value.ConventionName);
        return Task.FromResult<ContentState<MainPageModel>>(viewModel);
    }
}
