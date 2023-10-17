namespace ConventionGradingSystem.Models.ExpertFeedbackForm;

public record FormModel(
    IReadOnlyCollection<Grade> Grades,
    string Note);
