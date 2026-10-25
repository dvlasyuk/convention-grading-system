namespace ConventionGradingSystem.Host.Components.Administration.Contests.Models;

public record class ExpertFeedbacksListModel(
    ExpertFeedbacksListModel.EventModel Event,
    IReadOnlyCollection<ExpertFeedbacksListModel.FeedbackModel> Feedbacks,
    string? SearchFilter)
{
    public string? SearchFilter { get; set; } = SearchFilter;

    public record EventModel(
        string EventName,
        string ContestName,
        bool IsGraded,
        int FeedbacksQuantity,
        IReadOnlyList<CriterionModel> GradeCriterions);

    public record CriterionModel(
        string Identifier,
        string Name,
        float TotalGrade);

    public record FeedbackModel(
        int Number,
        string ExpertName,
        IReadOnlyDictionary<string, int> Grades,
        string? Note,
        string ReceivedAt);
}
