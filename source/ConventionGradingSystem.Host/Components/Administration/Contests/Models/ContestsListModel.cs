namespace ConventionGradingSystem.Host.Components.Administration.Contests.Models;

public record class ContestsListModel(
    IReadOnlyCollection<ContestsListModel.ContestModel> Contests,
    string? SearchFilter)
{
    public string? SearchFilter { get; set; } = SearchFilter;

    public record ContestModel(
        int Number,
        string Identifier,
        string Name,
        bool ExpertGrading,
        bool ParticipantGrading,
        bool RegisteredGrading,
        bool FriendlyGrading,
        bool AttendanceControl,
        IReadOnlyCollection<string> BannedTeams,
        int EventsQuantity,
        int ExpertFeedbaksQuantity,
        int ParticiapntFeedbaksQuantity);
}
