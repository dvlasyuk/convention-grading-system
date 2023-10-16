namespace ConventionGradingSystem.Database.Entities;

public class ExpertFeedback
{
    public Guid Identifier { get; set; }
    public required string EventId { get; set; }
    public required string? Note { get; set; }
    public ICollection<ExpertGrade> Grades { get; } = new List<ExpertGrade>();
}
