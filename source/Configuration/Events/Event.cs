namespace ConventionGradingSystem.Configuration.Events;

public class Event
{
    public int Identifier { get; set; }
    public string Name { get; set; }
    public ICollection<string> Participants { get; } = new List<string>();
}
