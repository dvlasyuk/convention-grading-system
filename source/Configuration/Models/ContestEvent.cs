namespace ConventionGradingSystem.Configuration.Models;

public class ContestEvent
{
    public string Identifier { get; set; } = string.Empty;
    public string Name { get; set; } = "Неизвестное мероприятие";
    public ICollection<string> Participants { get; } = new List<string>();
}
