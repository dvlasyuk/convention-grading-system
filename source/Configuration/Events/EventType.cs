namespace ConventionGradingSystem.Configuration.Events;

public class EventType
{
    public int Identifier { get; set; } = int.MinValue;
    public string Name { get; set; } = "Неизвестная категория";
    public ICollection<GradeType> ExpertGrades { get; } = new List<GradeType>();
    public ICollection<GradeType> ParticipantGrades { get; } = new List<GradeType>();
    public ICollection<Event> Events { get; } = new List<Event>();
}
