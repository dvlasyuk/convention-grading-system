namespace ConventionGradingSystem.Models.EventsList;

public record ViewModel(
    string ContestId,
    string ContestName,
    IReadOnlyList<GradeCriterion> ExpertCriterions,
    IReadOnlyList<GradeCriterion> ParticipantCriterions,
    IReadOnlyList<ContestEvent> Events);

public record GradeCriterion(string Identifier, string Name);

public record ContestEvent(
    string Identifier,
    string Name,
    int ExprertGradesQuantity,
    int ParticipantGradesQuantity,
    IReadOnlyDictionary<string, float> ExpertGrades,
    IReadOnlyDictionary<string, float> ParticipantGrades,
    float TotalExpertGrade,
    float TotalParticipantGrade,
    float TotalGrade,
    bool WithParticipants);
