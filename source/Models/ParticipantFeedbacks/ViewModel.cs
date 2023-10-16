namespace ConventionGradingSystem.Models.ParticipantFeedbacks;

public record ViewModel(
    string ContestName,
    string EventName,
    IReadOnlyList<GradeCriterion> Criterions,
    IReadOnlyList<Feedback> Feedbacks);

public record GradeCriterion(string Identifier, string Name);

public record Feedback(IReadOnlyDictionary<string, int> Grades, string? Note);
