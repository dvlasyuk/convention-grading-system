namespace ConventionGradingSystem.DataAccess.Database.Entities;

public class ParticipantFeedback
{
    public Guid Identifier { get; set; }

    public required string EventId { get; set; }

    public required string ParticipantId { get; set; }

    public required string? Note { get; set; }

    public required DateTimeOffset ReceivedAt { get; set; }

    public ICollection<ParticipantGrade> Grades { get; } = [];
}
