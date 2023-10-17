namespace ConventionGradingSystem.Models.ParticipantFeedbackForm;

public record GradeCriterion(
    string Identifier,
    string Name,
    string Description,
    int MinimalGrade,
    int MaximalGrade);
