using System.Diagnostics.CodeAnalysis;

namespace ConventionGradingSystem.Host.Components.Grading.Votings.Models;

[SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
public record class ParticipantVotingFormModel(
    ParticipantVotingFormModel.VotingModel Voting,
    ParticipantVotingFormModel.FormModel Form,
    bool IsFormDisabled,
    bool IsFormValid)
{
    public bool IsFormDisabled { get; set; } = IsFormDisabled;
    public bool IsFormValid { get; set; } = IsFormValid;

    public record VotingModel(
        string Name,
        bool BrigadeFriendlyVoting,
        bool TeamFriendlyVoting,
        int VotesQuantity,
        IReadOnlyCollection<string> BannedTeams,
        IReadOnlyCollection<CandidateModel> Candidates);

    public record CandidateModel(
        string Identifer,
        string Name,
        IReadOnlyCollection<string> Brigades,
        IReadOnlyCollection<string> Teams)
    {
        public override string ToString() => Name;
    }

    public record FormModel(
        TeamModel? SelectedTeam,
        ParticipantModel? SelectedParticipant,
        CandidateModel?[] SelectedCandidates,
        string? EnteredNote)
    {
        public TeamModel? SelectedTeam { get; set; } = SelectedTeam;
        public ParticipantModel? SelectedParticipant { get; set; } = SelectedParticipant;
        public string? EnteredNote { get; set; } = EnteredNote;
    }

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
}
