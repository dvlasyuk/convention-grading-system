using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.Host.Components.Shared.Models;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Layouts.Sections;

public partial class ContestsLayout
{
    private readonly IOptions<ApplicationConfiguration> _configuration;
    private readonly ContestsLayoutModel _viewModel = new();

    public ContestsLayout(IOptions<ApplicationConfiguration> configuration)
    {
        _configuration = configuration;
    }

    protected override void OnInitialized()
    {
        _viewModel.ContestsSectionAvailable = _configuration.Value.Contests.Count > 0;
    }
}
