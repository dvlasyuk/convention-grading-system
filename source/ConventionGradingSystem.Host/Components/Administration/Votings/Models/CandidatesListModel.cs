namespace ConventionGradingSystem.Host.Components.Administration.Votings.Models;

public record class CandidatesListModel
{
    public VotingModel? Voting { get; set; }
    public IEnumerable<CandidateModel> Candidates { get; set; } = [];
    public string? SearchFilter { get; set; }

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
