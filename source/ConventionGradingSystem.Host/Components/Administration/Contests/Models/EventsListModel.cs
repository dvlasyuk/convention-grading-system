namespace ConventionGradingSystem.Host.Components.Administration.Contests.Models;

public record class EventsListModel
{
    public ContestModel? Contest { get; set; }
    public IEnumerable<EventModel> Events { get; set; } = [];
    public string? SearchFilter { get; set; }

    public record ContestModel(
        string Name,
        bool RegisteredGrading,
        bool FriendlyGrading,
        bool AttendanceControl,
        int ExpertCriterionsQuantity,
        int ParticipantCriterionsQuantity,
        int EventsQuantity,
        int ExpertFeedbacksQuantity,
        int ParticipantFeedbacksQuantity);

    public record EventModel(
        int Number,
        string Identifier,
        string Name,
        IReadOnlyCollection<string> Brigades,
        int ExprertFeedbacksQuantity,
        int ParticipantFeedbacksQuantity,
        float ExpertGrade,
        float ParticipantGrade,
        float TotalGrade,
        string ExpertFeedbackFormLink,
        string ExpertFeedbacksListLink,
        string ParticipantFeedbackFormLink,
        string ParticipantFeedbacksListLink,
        string FacilitatorAttendanceFormLink);
}
