namespace ConventionGradingSystem.Host.Components.Grading.Contests.Models;

public record class FacilitatorAttendanceFormModel(
    FacilitatorAttendanceFormModel.EventModel Event,
    FacilitatorAttendanceFormModel.FormModel Form,
    string? SearchFilter)
{
    public string? SearchFilter { get; set; } = SearchFilter;

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

    public record FormModel(
        Dictionary<string, bool> AttendanceMarks,
        Dictionary<string, bool> SpecialMarks);
}
