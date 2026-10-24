using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.Host.Components.Shared.Models;
using ConventionGradingSystem.Host.Configuration;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Pages;

public partial class MainPage
{
    private readonly IOptions<ApplicationConfiguration> _applicationConfiguration;
    private readonly IOptions<DataConfiguration> _dataConfiguration;
    private readonly MainPageModel _viewModel = new();

    public MainPage(
        IOptions<ApplicationConfiguration> applicationConfiguration,
        IOptions<DataConfiguration> dataConfiguration)
    {
        _applicationConfiguration = applicationConfiguration;
        _dataConfiguration = dataConfiguration;
    }

    protected override void OnInitialized()
    {
        _viewModel.ConventionName = _applicationConfiguration.Value.ConventionName;

        _viewModel.ContestsSectionAvailable = _dataConfiguration.Value.Contests.Count > 0;
        _viewModel.VotingsSectionAvailable = _dataConfiguration.Value.Votings.Count > 0;
        _viewModel.TeamsSectionAvailable =
            _dataConfiguration.Value.Teams.Count > 0 &&
            _dataConfiguration.Value.Contests.Count > 0;
    }
}
