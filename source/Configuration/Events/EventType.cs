namespace ConventionGradingSystem.Configuration.Events;

public class EventType
{
    public int Identifier { get; set; }
    public string Name { get; set; }
    public ICollection<GradeType> ExpertGrades { get; } = new List<GradeType>();
    public ICollection<GradeType> ParticipantGrades { get; } = new List<GradeType>();
    public ICollection<Event> Events { get; } = new List<Event>();
}
