namespace ConventionGradingSystem.Host.Components.Administration.Votings.Models;

public record CandidateCardModel(
    string CandidateName,
    string VotingName,
    IReadOnlyCollection<string> Brigades,
    IReadOnlyCollection<string> Teams);
