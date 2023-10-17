namespace ConventionGradingSystem.Models.ParticipantFeedbackForm;

public record ViewModel(
    string ContestName,
    string EventName,
    IReadOnlyList<GradeCriterion> Criterions);
