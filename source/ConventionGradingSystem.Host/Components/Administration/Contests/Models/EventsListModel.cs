namespace ConventionGradingSystem.Host.Components.Administration.Contests.Models;

public record class EventsListModel(
    EventsListModel.ContestModel Contest,
    IReadOnlyCollection<EventsListModel.EventModel> Events,
    string? SearchFilter)
{
    public string? SearchFilter { get; set; } = SearchFilter;

    public record ContestModel(
        string Identifier,
        string Name,
        bool ExpertGrading,
        bool ParticipantGrading,
        bool RegisteredGrading,
        bool FriendlyGrading,
        bool AttendanceControl,
        IReadOnlyCollection<string> BannedTeams);

    public record EventModel(
        int Number,
        string Identifier,
        string Name,
        bool ParticipantsRegistered,
        IReadOnlyCollection<string> Brigades,
        int ExprertFeedbacksQuantity,
        int ParticipantFeedbacksQuantity,
        float ExpertGrade,
        float ParticipantGrade,
        float TotalGrade);
}
