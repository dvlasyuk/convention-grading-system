namespace ConventionGradingSystem.Host.Components.Administration.Contests.Models;

public record class EventsListModel(
    EventsListModel.ContestModel Contest,
    IReadOnlyCollection<EventsListModel.EventModel> Events,
    string? SearchFilter)
{
    public string? SearchFilter { get; set; } = SearchFilter;

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
        float TotalGrade);
}
