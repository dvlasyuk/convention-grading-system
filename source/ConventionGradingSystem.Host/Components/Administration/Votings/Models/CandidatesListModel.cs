namespace ConventionGradingSystem.Host.Components.Administration.Votings.Models;

public record class CandidatesListModel(
    CandidatesListModel.VotingModel Voting,
    IReadOnlyCollection<CandidatesListModel.CandidateModel> Candidates,
    string? SearchFilter)
{
    public string? SearchFilter { get; set; } = SearchFilter;

    public record VotingModel(
        string Name,
        bool BrigadeFriendlyVoting,
        bool TeamFriendlyVoting,
        int VotesQuantity,
        int CandidatesQuantity,
        int TotalVotesQuantity);

    public record CandidateModel(
        int Number,
        string Identifier,
        string Name,
        IReadOnlyCollection<string> Brigades,
        IReadOnlyCollection<string> Teams,
        int VotesQuantity,
        string VotesListLink);
}
