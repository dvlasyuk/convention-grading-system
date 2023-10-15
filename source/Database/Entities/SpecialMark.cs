namespace ConventionGradingSystem.Database.Entities;

public class SpecialMark
{
    public required string ParticipantId { get; set; }
    public required int ContestId { get; set; }
    public required int EventId { get; set; }
}
