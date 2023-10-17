namespace ConventionGradingSystem.Models.EventAttendanceForm;

public record ViewModel(
    string ContestName,
    string EventName,
    IReadOnlyCollection<Participant> Participants);
