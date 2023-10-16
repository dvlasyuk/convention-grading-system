namespace ConventionGradingSystem.Database.Entities;

public class ExpertNote
{
    public int Identifier { get; set; }
    public required string ContestId { get; set; }
    public required string EventId { get; set; }
    public required string Note { get; set; }
}
