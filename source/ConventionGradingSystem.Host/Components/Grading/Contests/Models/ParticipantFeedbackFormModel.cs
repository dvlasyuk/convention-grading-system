namespace ConventionGradingSystem.Host.Components.Grading.Contests.Models;

public record class ParticipantFeedbackFormModel
{
    public EventModel? Event { get; set; }
    public bool IsFormDisabled { get; set; }
    public bool IsFormValid { get; set; }

    public TeamModel? SelectedTeam { get; set; }
    public ParticipantModel? SelectedParticipant { get; set; }
    public Dictionary<string, int> SelectedGrades { get; } = [];
    public string? EnteredNote { get; set; }

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

    public record TeamModel(string Identifier, string Name)
    {
        public override string ToString() => Name;
    }

    public record ParticipantModel(string Identifier, string Name)
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
