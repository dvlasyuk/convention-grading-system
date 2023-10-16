namespace ConventionGradingSystem.Models.EventsList;

public record ViewModel(
    string ContestName,
    IReadOnlyList<GradeCriterion> ExpertCriterions,
    IReadOnlyList<GradeCriterion> ParticipantCriterions,
    IReadOnlyList<ContestEvent> Events);

public record GradeCriterion(string Identifier, string Name);

public record ContestEvent(
    string Identifier,
    string Name,
    int ExprertFeedbacksQuantity,
    int ParticipantFeedbacksQuantity,
    IReadOnlyDictionary<string, float> ExpertGrades,
    IReadOnlyDictionary<string, float> ParticipantGrades,
    float TotalExpertGrade,
    float TotalParticipantGrade,
    float TotalGrade,
    bool WithParticipants);
