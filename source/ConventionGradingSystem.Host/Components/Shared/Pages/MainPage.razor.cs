using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.Host.Components.Shared.Models;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Pages;

public partial class MainPage
{
    private readonly IOptions<ApplicationConfiguration> _configuration;
    private readonly MainPageModel _viewModel = new();

    public MainPage(IOptions<ApplicationConfiguration> configuration)
    {
        _configuration = configuration;
    }

    protected override void OnInitialized()
    {
        _viewModel.ConventionName = _configuration.Value.ConventionName;

        _viewModel.ContestsSectionAvailable = _configuration.Value.Contests.Count > 0;
        _viewModel.VotingsSectionAvailable = _configuration.Value.Votings.Count > 0;
        _viewModel.TeamsSectionAvailable =
            _configuration.Value.Teams.Count > 0 &&
            _configuration.Value.Contests.Count > 0;
    }
}
