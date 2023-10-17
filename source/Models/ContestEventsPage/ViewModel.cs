namespace ConventionGradingSystem.Models.ContestEventsPage;

public record ViewModel(
    string ContestName,
    IReadOnlyList<GradeCriterion> ExpertCriterions,
    IReadOnlyList<GradeCriterion> ParticipantCriterions,
    IReadOnlyList<ContestEvent> Events);
