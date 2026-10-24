namespace ConventionGradingSystem.Host.Configuration;

public class ApplicationConfiguration
{
    public string ConventionName { get; set; } = "Неизвестный слёт";

    public bool ContestsFeatureEnabled { get; set; }

    public bool VotingsFeatureEnabled { get; set; }

    public bool TeamsFeatureEnabled { get; set; }
}
