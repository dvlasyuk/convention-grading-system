namespace ConventionGradingSystem.Host.Components.Grading.Contests.Models;

public record class ExpertFeedbackFormModel
{
    public EventModel? Event { get; set; }
    public bool IsFormDisabled { get; set; }
    public bool IsFormValid { get; set; }

    public ExpertModel? SelectedExpert { get; set; }
    public Dictionary<string, int> SelectedGrades { get; } = [];
    public string? EnteredNote { get; set; }

    public record EventModel(
        string EventName,
        string ContestName,
        bool IsGraded,
        IReadOnlyList<CriterionModel> Criterions);

    public record ExpertModel(string Identifier, string Name)
    {
        public override string ToString() => Name;
    }

    public record CriterionModel(
        string Identifier,
        string Name,
        string Description,
        int MinimalGrade,
        int MaximalGrade);
}
