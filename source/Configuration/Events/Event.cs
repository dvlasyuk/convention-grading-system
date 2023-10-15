using System.Diagnostics.CodeAnalysis;

namespace ConventionGradingSystem.Configuration.Events;

[SuppressMessage("Naming", "CA1716:Identifiers should not match keywords")]
public class Event
{
    public int Identifier { get; set; } = int.MinValue;
    public string Name { get; set; } = "Неизвестное мероприятие";
    public ICollection<string> Participants { get; } = new List<string>();
}
