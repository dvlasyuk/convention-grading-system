namespace ConventionGradingSystem.Models.EventsList;

public record ViewModel(
    int EventTypeId,
    string EventTypeName,
    IReadOnlyCollection<GradeType> ExpertGradeTypes,
    IReadOnlyCollection<GradeType> ParticipantGradeTypes,
    IReadOnlyCollection<Event> Events);

public record GradeType(int Identifier, string Name);

public record Event(
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
