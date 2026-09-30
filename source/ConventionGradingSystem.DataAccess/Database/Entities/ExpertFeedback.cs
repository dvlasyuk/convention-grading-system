namespace ConventionGradingSystem.DataAccess.Database.Entities;

public class ExpertFeedback
{
    public Guid Identifier { get; set; }

    public required string EventId { get; set; }

    public required string ExpertId { get; set; }

    public required string? Note { get; set; }

    public required DateTimeOffset ReceivedAt { get; set; }

    public ICollection<ExpertGrade> Grades { get; } = [];
}
