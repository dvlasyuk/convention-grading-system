using ConventionGradingSystem.Configuration.Events;
using ConventionGradingSystem.Configuration.Participants;

namespace ConventionGradingSystem.Configuration;

public class ApplicationConfiguration
{
    public List<EventType> EventTypes { get; set; }
    public List<Participant> Participants { get; set; }
}
