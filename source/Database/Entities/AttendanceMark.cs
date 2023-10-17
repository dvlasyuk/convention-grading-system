namespace ConventionGradingSystem.Database.Entities;

public class AttendanceMark
{
    public required string ParticipantId { get; set; }
    public required string EventId { get; set; }
    public required bool SpecialMark { get; set; }
}
