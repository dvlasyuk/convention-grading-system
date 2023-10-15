namespace ConventionGradingSystem.Database.Entities;

public class ParticipationMark
{
    public required string ParticipantId { get; set; }
    public required int EventTypeId { get; set; }
    public required int EventId { get; set; }
}
