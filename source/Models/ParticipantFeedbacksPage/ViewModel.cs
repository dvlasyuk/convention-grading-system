namespace ConventionGradingSystem.Models.ParticipantFeedbacksPage;

public record ViewModel(
    string ContestName,
    string EventName,
    IReadOnlyList<GradeCriterion> Criterions,
    IReadOnlyList<Feedback> Feedbacks);
