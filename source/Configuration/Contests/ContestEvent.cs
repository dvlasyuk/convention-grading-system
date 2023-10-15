namespace ConventionGradingSystem.Configuration.Contests;

public class ContestEvent
{
    public int Identifier { get; set; } = int.MinValue;
    public string Name { get; set; } = "Неизвестное мероприятие";
    public ICollection<string> Participants { get; } = new List<string>();
}
