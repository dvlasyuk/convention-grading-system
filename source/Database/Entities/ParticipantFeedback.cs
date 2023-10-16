namespace ConventionGradingSystem.Database.Entities;

public class ParticipantFeedback
{
    public Guid Identifier { get; set; }
    public required string EventId { get; set; }
    public required string? Note { get; set; }
    public ICollection<ParticipantGrade> Grades { get; } = new List<ParticipantGrade>();
}
