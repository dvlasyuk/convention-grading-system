namespace ConventionGradingSystem.Models.ParticipantGrade;

public record ViewModel(
    string EventTypeName,
    string EventName,
    IReadOnlyList<GradeType> GradeTypes);

public record GradeType(
    int Identifier,
    string Name,
    string? Description,
    int MinimalGrade,
    int MaximalGrade);
