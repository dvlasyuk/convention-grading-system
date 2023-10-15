namespace ConventionGradingSystem.Database.Entities;

public class ParticipationMark
{
    public string ParticipantId { get; set; }
    public int EventTypeId { get; set; }
    public int EventId { get; set; }
}
