namespace ConventionGradingSystem.Host.Components.Administration.Votings.Models;

public record class VotingCardParameters(
    string VotingName,
    bool BrigadeFriendlyVoting,
    bool TeamFriendlyVoting,
    int VotesQuantity,
    IReadOnlyCollection<string> BannedTeams);
