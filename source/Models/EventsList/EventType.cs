namespace ConventionGradingSystem.Models.EventsList;

public class EventType
{
    public int Identifier { get; set; }
    public string Name { get; set; }
    public List<GradeType> ExpertGrades { get; set; }
    public List<GradeType> ParticipantGrades { get; set; }
}
