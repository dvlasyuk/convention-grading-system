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
        int CandidatesQuantity,
        int TotalVotesQuantity,
        string CandidatesListLink,
        string ParticipantVotingFormLink);
}
