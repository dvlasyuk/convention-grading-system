namespace ConventionGradingSystem.Models.ParticipantFeedbackForm;

public record ViewModel(
    string ContestName,
    string EventName,
    IReadOnlyList<GradeCriterion> Criterions);

public record GradeCriterion(
    string Identifier,
    string Name,
    string Description,
    int MinimalGrade,
    int MaximalGrade);
