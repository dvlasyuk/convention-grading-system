namespace ConventionGradingSystem.Models.ExpertFeedbackForm;

public record ViewModel(
    string ContestName,
    string EventName,
    IReadOnlyList<GradeCriterion> Criterions);
