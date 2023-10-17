namespace ConventionGradingSystem.Models.EventAttendanceForm;

public record FormModel(
    IReadOnlyCollection<string>? AttendanceMarks,
    IReadOnlyCollection<string>? SpecialMarks);
