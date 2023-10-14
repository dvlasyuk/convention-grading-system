namespace ConventionGradingSystem.Models.EventParticipants
{
    public class Participant
    {
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Brigade { get; set; }
        public string Team { get; set; }
        public bool ParticipitionMark { get; set; }
        public bool SpecialMark { get; set; }
    }
}
