namespace ConventionGradingSystem.Models.EventAttendanceForm;

public record Participant(
    string Identifier,
    string Name,
    string Brigade,
    bool AttendanceMark,
    bool SpecialMark);
