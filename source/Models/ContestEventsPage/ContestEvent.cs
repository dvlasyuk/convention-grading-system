namespace ConventionGradingSystem.Models.ContestEventsPage;

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
