namespace ConventionGradingSystem.Host.Components.Administration.Votings.Models;

public record class VotesListModel
{
    public CandidateModel? Candidate { get; set; }
    public IEnumerable<VoteModel> Votes { get; set; } = [];
    public string? SearchFilter { get; set; }

    public record CandidateModel(
        string CandidateName,
        string VotingName,
        IReadOnlyCollection<string> Brigades,
        int VotesQuantity);

    public record VoteModel(
        int Number,
        string ParticipantName,
        string? ParticipantBrigade,
        string ParticipantTeam,
        string? Note,
        string ReceivedAt);
}
