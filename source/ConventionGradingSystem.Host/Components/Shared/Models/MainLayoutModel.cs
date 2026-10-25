namespace ConventionGradingSystem.Host.Components.Shared.Models;

public record class MainLayoutModel(
    string ConventionName,
    bool IsDrawerOpen)
{
    public bool IsDrawerOpen { get; set; } = IsDrawerOpen;
}
