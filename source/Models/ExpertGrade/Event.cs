namespace ConventionGradingSystem.Models.ExpertGrade;

public class Event
{
    public string EventTypeName { get; set; }
    public string EventName { get; set; }
    public List<GradeType> GradeTypes { get; set; }
}
