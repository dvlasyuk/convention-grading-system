using System.Globalization;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.DataAccess.Database.Entities;
using ConventionGradingSystem.Host.Navigation;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;

using MudBlazor;

namespace ConventionGradingSystem.Host.Components.Grading.Votings.Pages;

public partial class ParticipantVotingForm
{
    private readonly DatabaseContext _databaseContext;
    private readonly IOptions<ApplicationConfiguration> _configuration;
    private readonly NavigationManager _navigationManager;
    private readonly ProtectedLocalStorage _localStorage;

    private VotingDetails? _votingDetails;
    private bool _isFormDisabled;
    private bool _isFormValid;

    private Team? _selectedTeam;
    private Participant? _selectedParticipant;
    private Candidate?[] _selectedCandidates = [];
    private string? _enteredNote;

    private MudForm? _formSelector;
    private MudAutocomplete<Participant>? _participantSelector;
    private readonly List<MudSelect<Candidate>> _candidateSelectors = [];
    private MudSelect<Candidate> CandidateSelector { set => _candidateSelectors.Add(value); }

    [Parameter]
    public string? VotingId { get; set; }

    public ParticipantVotingForm(
        DatabaseContext databaseContext,
        IOptions<ApplicationConfiguration> configuration,
        NavigationManager navigationManager,
        ProtectedLocalStorage localStorage)
    {
        _databaseContext = databaseContext;
        _configuration = configuration;
        _navigationManager = navigationManager;
        _localStorage = localStorage;
    }

    protected override async Task OnInitializedAsync()
    {
        var voting = _configuration.Value.Votings.FirstOrDefault(item => item.Identifier == VotingId);
        if (voting == null)
        {
            return;
        }

        var result = await _localStorage.GetAsync<string>(GetLocalStorageKey());
        _isFormDisabled = result.Success;

        _votingDetails = new VotingDetails(
            Name: voting.Name,
            BrigadeFriendlyVoting: voting.BrigadeFriendlyVoting,
            TeamFriendlyVoting: voting.TeamFriendlyVoting,
            VotesQuantity: voting.VotesQuantity,
            BannedTeams: [.. voting.BannedTeams],
            Candidates: [.. voting.Candidates
                .Select(candidate => new Candidate(
                    Identifer: candidate.Identifier,
                    Name: candidate.Name,
                    Brigades: [.. candidate.Brigades],
                    Teams: [.. candidate.Teams]))]);

        _selectedCandidates = new Candidate?[_votingDetails.VotesQuantity];
    }

    private async Task ProcessForm()
    {
        await _formSelector!.ValidateAsync();
        if (!_isFormValid)
        {
            return;
        }

        _databaseContext.ParticipantVotes.AddRange(_selectedCandidates.Select(candidate => new ParticipantVote
        {
            ParticipantId = _selectedParticipant!.Identifier,
            CandidateId = candidate!.Identifer,
            Note = _enteredNote,
            ReceivedAt = DateTimeOffset.Now
        }));

        await _databaseContext.SaveChangesAsync();
        await _localStorage.SetAsync(
            key: GetLocalStorageKey(),
            value: DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));

        _navigationManager.NavigateTo(
            uri: NavigationHelper.Grading.GetParticipantVotingFormLink(VotingId!),
            forceLoad: true);
    }

    private Task<IEnumerable<Team>> SearchTeam(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Task.FromResult(Enumerable.Empty<Team>());
        }

        return Task.FromResult<IEnumerable<Team>>([.. _configuration.Value.Teams
            .Where(team => !_votingDetails!.BannedTeams.Contains(team.Identifier))
            .Where(team => team.Name.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Select(team => new Team(
                Identifier: team.Identifier,
                Name: team.Name))]);
    }

    private Task<IEnumerable<Participant>> SearchParticipant(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Task.FromResult(Enumerable.Empty<Participant>());
        }
        if (_selectedTeam == null)
        {
            return Task.FromResult(Enumerable.Empty<Participant>());
        }

        return Task.FromResult<IEnumerable<Participant>>([.. _configuration.Value.Teams
            .Where(team => team.Identifier == _selectedTeam.Identifier)
            .SelectMany(team => _configuration.Value.Participants
                .Where(participant => participant.Team == team.Identifier))
            .Where(participant => participant.Name.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Select(participant => new Participant(
                Identifier: participant.Identifier,
                Name: participant.Name,
                Brigade: participant.Brigade,
                Team: participant.Team))]);
    }

    private string? ValidateSelectedCandidate(int index, Candidate value)
    {
        if (value == null)
        {
            return null;
        }

        var friendlyBrigadeSelected =
            !string.IsNullOrWhiteSpace(_selectedParticipant!.Brigade) &&
            value.Brigades.Contains(_selectedParticipant!.Brigade);

        if (!_votingDetails!.BrigadeFriendlyVoting && friendlyBrigadeSelected)
        {
            return "Нельзя голосовать за кандидата, связанного с твоим отрядом";
        }
        if (!_votingDetails!.TeamFriendlyVoting && value.Teams.Contains(_selectedParticipant!.Team))
        {
            return "Нельзя голосовать за кандидата, связанного с твоей командой";
        }
        if (index > 0 && _selectedCandidates.Take(index).Contains(value))
        {
            return "Нельзя дважды проголосовать за одного и того же кандидата";
        }

        return null;
    }

    private async Task OnSelectedTeamChange()
    {
        await _participantSelector!.ResetAsync();
    }

    private async Task OnSelectedParticipantChange()
    {
        foreach (var selector in _candidateSelectors)
        {
            await selector.ResetAsync();
        }
    }

    private async Task OnSelectedCandidateChange(int index)
    {
        for (var number = _candidateSelectors.Count - 1; number > index; number--)
        {
            await _candidateSelectors[number].ResetAsync();
        }
    }

    private bool DisableCandidateSelector(int index)
    {
        if (_selectedParticipant == null)
        {
            return true;
        }
        if (index == 0)
        {
            return false;
        }

        return _selectedCandidates[index - 1] == null;
    }

    private string GetLocalStorageKey() => $"AudienceVoting-{VotingId}";

    private sealed record VotingDetails(
        string Name,
        bool BrigadeFriendlyVoting,
        bool TeamFriendlyVoting,
        int VotesQuantity,
        IReadOnlyCollection<string> BannedTeams,
        IReadOnlyCollection<Candidate> Candidates);

    private sealed record Team(string Identifier, string Name)
    {
        public override string ToString() => Name;
    }

    private sealed record Participant(
        string Identifier,
        string Name,
        string? Brigade,
        string Team)
    {
        public override string ToString() => Name;
    }

    private sealed record Candidate(
        string Identifer,
        string Name,
        IReadOnlyCollection<string> Brigades,
        IReadOnlyCollection<string> Teams)
    {
        public override string ToString() => Name;
    }
}
