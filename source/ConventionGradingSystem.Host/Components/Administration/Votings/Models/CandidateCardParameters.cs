namespace ConventionGradingSystem.Host.Components.Administration.Votings.Models;

public record CandidateCardParameters(
    string CandidateName,
    string VotingName,
    IReadOnlyCollection<string> Brigades,
    IReadOnlyCollection<string> Teams);
