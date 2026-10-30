namespace ConventionGradingSystem.Host.Components.Administration.Votings.Models;

public record class VotingsListModel(
    IReadOnlyCollection<VotingsListModel.VotingModel> Votings,
    string? SearchFilter)
{
    public string? SearchFilter { get; set; } = SearchFilter;

    public record VotingModel(
        int Number,
        string Identifier,
        string Name,
        bool BrigadeFriendlyVoting,
        bool TeamFriendlyVoting,
        int VotesQuantity,
        IReadOnlyCollection<string> BannedTeams,
        int CandidatesQuantity,
        int TotalVotesQuantity);
}
