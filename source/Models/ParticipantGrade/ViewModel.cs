namespace ConventionGradingSystem.Models.ParticipantGrade;

public record ViewModel(
    string ContestName,
    string EventName,
    IReadOnlyList<GradeCriterion> Criterions);

public record GradeCriterion(
    int Identifier,
    string Name,
    string? Description,
    int MinimalGrade,
    int MaximalGrade);
