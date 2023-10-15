namespace ConventionGradingSystem.Database.Entities;

public class ExpertNote
{
    public int Identifier { get; set; }
    public required int ContestId { get; set; }
    public required int EventId { get; set; }
    public required string Note { get; set; }
}
