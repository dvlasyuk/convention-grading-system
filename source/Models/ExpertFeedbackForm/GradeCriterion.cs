namespace ConventionGradingSystem.Models.ExpertFeedbackForm;

public record GradeCriterion(
    string Identifier,
    string Name,
    string Description,
    int MinimalGrade,
    int MaximalGrade);
