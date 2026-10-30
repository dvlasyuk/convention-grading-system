namespace ConventionGradingSystem.Host.Components.Administration.Votings.Models;

public record class VotesListModel(
    VotesListModel.CandidateModel Candidate,
    IReadOnlyCollection<VotesListModel.VoteModel> Votes,
    string? SearchFilter)
{
    public string? SearchFilter { get; set; } = SearchFilter;

    public record CandidateModel(
        string CandidateName,
        string VotingName,
        IReadOnlyCollection<string> Brigades,
        IReadOnlyCollection<string> Teams);

    public record VoteModel(
        int Number,
        string ParticipantName,
        string? ParticipantBrigade,
        string ParticipantTeam,
        string? Note,
        string ReceivedAt);
}
