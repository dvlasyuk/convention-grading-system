namespace ConventionGradingSystem.Host.Components.Grading.Contests.Models;

public record class FacilitatorAttendanceFormModel
{
    public EventModel? Event { get; set; }
    public string? SearchFilter { get; set; }

    public Dictionary<string, bool> AttendanceMarks { get; } = [];
    public Dictionary<string, bool> SpecialMarks { get; } = [];

    public record EventModel(
        string EventName,
        string ContestName,
        bool AttendanceControl,
        IReadOnlyCollection<ParticipantModel> Participants);

    public record ParticipantModel(
        string Identifier,
        string Name,
        string? Brigade,
        string Team);
}
