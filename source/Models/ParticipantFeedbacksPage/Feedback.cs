namespace ConventionGradingSystem.Models.ParticipantFeedbacksPage;

public record Feedback(IReadOnlyDictionary<string, int> Grades, string? Note);
