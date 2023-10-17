namespace ConventionGradingSystem.Models.ExpertFeedbacksPage;

public record Feedback(IReadOnlyDictionary<string, int> Grades, string? Note);
