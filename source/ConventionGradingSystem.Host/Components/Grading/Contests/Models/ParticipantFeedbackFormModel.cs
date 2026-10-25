namespace ConventionGradingSystem.Host.Components.Grading.Contests.Models;

public record class ParticipantFeedbackFormModel(
    ParticipantFeedbackFormModel.EventModel Event,
    ParticipantFeedbackFormModel.FormModel Form,
    bool IsFormDisabled,
    bool IsFormValid)
{
    public bool IsFormDisabled { get; set; } = IsFormDisabled;
    public bool IsFormValid { get; set; } = IsFormValid;

    public record EventModel(
        string EventName,
        string ContestName,
        bool IsGraded,
        bool RegisteredGrading,
        bool FriendlyGrading,
        IReadOnlyCollection<string> BannedTeams,
        IReadOnlyCollection<string> Brigades,
        IReadOnlyCollection<string> Participants,
        IReadOnlyList<CriterionModel> Criterions);

    public record CriterionModel(
        string Identifier,
        string Name,
        string Description,
        int MinimalGrade,
        int MaximalGrade);

    public record FormModel(
        TeamModel? SelectedTeam,
        ParticipantModel? SelectedParticipant,
        Dictionary<string, int> SelectedGrades,
        string? EnteredNote)
    {
        public TeamModel? SelectedTeam { get; set; } = SelectedTeam;
        public ParticipantModel? SelectedParticipant { get; set; } = SelectedParticipant;
        public string? EnteredNote { get; set; } = EnteredNote;
    }

    public record TeamModel(string Identifier, string Name)
    {
        public override string ToString() => Name;
    }

    public record ParticipantModel(string Identifier, string Name)
    {
        public override string ToString() => Name;
    }
}
