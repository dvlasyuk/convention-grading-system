namespace ConventionGradingSystem.Database.Entities;

public class ParticipantNote
{
    public int Identifier { get; set; }
    public int EventTypeId { get; set; }
    public int EventId { get; set; }
    public string Note { get; set; }
}
