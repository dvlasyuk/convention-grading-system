namespace ConventionGradingSystem.Host.Components.Shared.Models;

public record class MainPageModel
{
    public string ConventionName { get; set; } = string.Empty;

    public bool ContestsSectionAvailable { get; set; }
    public bool VotingsSectionAvailable { get; set; }
    public bool TeamsSectionAvailable { get; set; }
}
