namespace ConventionGradingSystem.Database.Entities;

public class ParticipantNote
{
    public int Identifier { get; set; }
    public required string ContestId { get; set; }
    public required string EventId { get; set; }
    public required string Note { get; set; }
}
