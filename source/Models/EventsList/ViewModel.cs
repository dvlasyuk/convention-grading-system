namespace ConventionGradingSystem.Models.EventsList;

public record ViewModel(
    int ContestId,
    string ContestName,
    IReadOnlyCollection<GradeCriterion> ExpertCriterions,
    IReadOnlyCollection<GradeCriterion> ParticipantCriterions,
    IReadOnlyCollection<ContestEvent> Events);

public record GradeCriterion(int Identifier, string Name);

public record ContestEvent(
    int Identifier,
    string Name,
    int ExprertGradesQuantity,
    int ParticipantGradesQuantity,
    IReadOnlyDictionary<int, float> ExpertGrades,
    IReadOnlyDictionary<int, float> ParticipantGrades,
    float TotalExpertGrade,
    float TotalParticipantGrade,
    float TotalGrade,
    bool WithParticipants);
