using ConventionGradingSystem.Configuration.Events;
using ConventionGradingSystem.Configuration.Participants;

namespace ConventionGradingSystem.Configuration;

public class ApplicationConfiguration
{
    public ICollection<EventType> EventTypes { get; } = new List<EventType>();
    public ICollection<Participant> Participants { get; } = new List<Participant>();
}
