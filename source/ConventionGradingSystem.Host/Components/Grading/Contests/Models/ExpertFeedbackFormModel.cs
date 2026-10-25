namespace ConventionGradingSystem.Host.Components.Grading.Contests.Models;

public record class ExpertFeedbackFormModel(
    ExpertFeedbackFormModel.EventModel Event,
    ExpertFeedbackFormModel.FormModel Form,
    bool IsFormDisabled,
    bool IsFormValid)
{
    public bool IsFormDisabled { get; set; } = IsFormDisabled;
    public bool IsFormValid { get; set; } = IsFormValid;

    public record EventModel(
        string EventName,
        string ContestName,
        bool IsGraded,
        IReadOnlyList<CriterionModel> Criterions);

    public record CriterionModel(
        string Identifier,
        string Name,
        string Description,
        int MinimalGrade,
        int MaximalGrade);

    public record FormModel(
        ExpertModel? SelectedExpert,
        Dictionary<string, int> SelectedGrades,
        string? EnteredNote)
    {
        public ExpertModel? SelectedExpert { get; set; } = SelectedExpert;
        public string? EnteredNote { get; set; } = EnteredNote;
    }

    public record ExpertModel(string Identifier, string Name)
    {
        public override string ToString() => Name;
    }
}
