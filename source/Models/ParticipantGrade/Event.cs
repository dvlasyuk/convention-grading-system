namespace ConventionGradingSystem.Models.ParticipantGrade;

public class Event
{
    public string EventTypeName { get; set; }
    public string EventName { get; set; }
    public List<GradeType> GradeTypes { get; set; }
}
