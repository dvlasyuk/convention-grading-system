namespace ConventionGradingSystem.Host.Components.Administration.Contests.Models;

public record class ParticipantFeedbacksListModel(
    ParticipantFeedbacksListModel.EventModel Event,
    IReadOnlyCollection<ParticipantFeedbacksListModel.FeedbackModel> Feedbacks,
    string? SearchFilter)
{
    public string? SearchFilter { get; set; } = SearchFilter;

    public record EventModel(
        string EventName,
        string ContestName,
        bool ParticipantGrading,
        bool ParticipantsRegistered,
        IReadOnlyCollection<string> Brigades,
        IReadOnlyList<CriterionModel> GradeCriterions);

    public record CriterionModel(string Identifier, string Name);

    public record FeedbackModel(
        int Number,
        string ParticipantName,
        string? ParticipantBrigade,
        string ParticipantTeam,
        IReadOnlyDictionary<string, int> Grades,
        string? Note,
        string ReceivedAt);
}
