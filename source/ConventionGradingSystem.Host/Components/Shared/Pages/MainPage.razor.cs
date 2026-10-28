using ConventionGradingSystem.Host.Components.Shared.Models;
using ConventionGradingSystem.Host.Configuration;
using ConventionGradingSystem.Host.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Pages;

public partial class MainPage
{
    private readonly NavigationManager _navigationManager;
    private readonly IOptions<ApplicationConfiguration> _applicationConfiguration;

    public MainPage(
        NavigationManager navigationManager,
        IOptions<ApplicationConfiguration> applicationConfiguration)
    {
        _navigationManager = navigationManager;
        _applicationConfiguration = applicationConfiguration;
    }

    protected override Task<ContentState<MainPageModel>> LoadContent()
    {
        var viewModel = new MainPageModel(_applicationConfiguration.Value.ConventionName);
        return Task.FromResult<ContentState<MainPageModel>>(viewModel);
    }

    private async Task SignInIntoSystem()
    {
        _navigationManager.NavigateTo(NavigationHelper.SignInPage());
    }
}
