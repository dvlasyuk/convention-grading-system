using System.Collections.Generic;

using EventGradingSystem.Configuration.Events;
using EventGradingSystem.Configuration.Participants;

namespace EventGradingSystem.Configuration
{
    public class ApplicationConfiguration
    {
        public List<EventType> EventTypes { get; set; }
        public List<Participant> Participants { get; set; }
    }
}
