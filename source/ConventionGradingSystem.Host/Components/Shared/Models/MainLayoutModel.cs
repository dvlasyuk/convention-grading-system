namespace ConventionGradingSystem.Host.Components.Shared.Models;

public record class MainLayoutModel
{
    public string ConventionName { get; set; } = string.Empty;
    public bool IsDrawerOpen { get; set; }
}
