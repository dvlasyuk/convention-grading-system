using ConventionGradingSystem.DataAccess.Configuration;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Shared.Layouts;

public partial class BaseLayout
{
    private readonly IOptions<ApplicationConfiguration> _configuration;

    private string _conventionName = string.Empty;

    public BaseLayout(IOptions<ApplicationConfiguration> configuration)
    {
        _configuration = configuration;
    }

    protected override void OnInitialized()
    {
        _conventionName = _configuration.Value.ConventionName;
    }
}
