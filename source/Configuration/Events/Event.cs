using System.Collections.Generic;

namespace EventGradingSystem.Configuration.Events
{
    public class Event
    {
        public int Identifier { get; set; }
        public string Name { get; set; }
        public List<string> Participants { get; set; }
    }
}
