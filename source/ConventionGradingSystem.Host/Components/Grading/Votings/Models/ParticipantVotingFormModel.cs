using System.Diagnostics.CodeAnalysis;

namespace ConventionGradingSystem.Host.Components.Grading.Votings.Models;

[SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
public record class ParticipantVotingFormModel
{
    public VotingModel? Voting { get; set; }
    public bool IsFormDisabled { get; set; }
    public bool IsFormValid { get; set; }

    public TeamModel? SelectedTeam { get; set; }
    public ParticipantModel? SelectedParticipant { get; set; }
    public CandidateModel?[] SelectedCandidates { get; set; } = [];
    public string? EnteredNote { get; set; }

    public record VotingModel(
        string Name,
        bool BrigadeFriendlyVoting,
        bool TeamFriendlyVoting,
        int VotesQuantity,
        IReadOnlyCollection<string> BannedTeams,
        IReadOnlyCollection<CandidateModel> Candidates);

    public record TeamModel(string Identifier, string Name)
    {
        public override string ToString() => Name;
    }

    public record ParticipantModel(
        string Identifier,
        string Name,
        string? Brigade,
        string Team)
    {
        public override string ToString() => Name;
    }

    public record CandidateModel(
        string Identifer,
        string Name,
        IReadOnlyCollection<string> Brigades,
        IReadOnlyCollection<string> Teams)
    {
        public override string ToString() => Name;
    }
}
