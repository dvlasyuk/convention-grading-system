namespace ConventionGradingSystem.Models.ParticipantFeedbackForm;

public record FormModel(
    IReadOnlyCollection<Grade> Grades,
    string Note);
