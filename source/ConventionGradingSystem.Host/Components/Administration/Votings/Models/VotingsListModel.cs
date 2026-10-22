namespace ConventionGradingSystem.Host.Components.Administration.Votings.Models;

public record class VotingsListModel
{
    public IEnumerable<VotingModel> Votings { get; set; } = [];
    public string? SearchFilter { get; set; }

    public record VotingModel(
        int Number,
        string Identifier,
        string Name,
        int CandidatesQuantity,
        int TotalVotesQuantity,
        string CandidatesListLink,
        string ParticipantVotingFormLink);
}
