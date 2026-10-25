using System.Globalization;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.DataAccess.Database.Entities;
using ConventionGradingSystem.Host.Components.Grading.Votings.Models;
using ConventionGradingSystem.Host.Components.Shared.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;

using MudBlazor;

namespace ConventionGradingSystem.Host.Components.Grading.Votings.Pages;

public partial class ParticipantVotingForm
{
    private readonly DatabaseContext _databaseContext;
    private readonly IOptions<DataConfiguration> _dataConfiguration;
    private readonly NavigationManager _navigationManager;
    private readonly ProtectedLocalStorage _localStorage;

    private MudForm? _formSelector;
    private MudAutocomplete<ParticipantVotingFormModel.ParticipantModel>? _participantSelector;
    private readonly List<MudSelect<ParticipantVotingFormModel.CandidateModel>> _candidateSelectors = [];
    private MudSelect<ParticipantVotingFormModel.CandidateModel> CandidateSelector { set => _candidateSelectors.Add(value); }

    [Parameter]
    public string? VotingId { get; set; }

    public ParticipantVotingForm(
        DatabaseContext databaseContext,
        IOptions<DataConfiguration> dataConfiguration,
        NavigationManager navigationManager,
        ProtectedLocalStorage localStorage)
    {
        _databaseContext = databaseContext;
        _dataConfiguration = dataConfiguration;
        _navigationManager = navigationManager;
        _localStorage = localStorage;
    }

    protected override async Task<ContentState<ParticipantVotingFormModel>> LoadContent()
    {
        var voting = _dataConfiguration.Value.Votings.FirstOrDefault(item => item.Identifier == VotingId);
        if (voting == null)
        {
            return Shared.Models.LoadingError.VotingNotFound;
        }

        var viewVoting = new ParticipantVotingFormModel.VotingModel(
            Name: voting.Name,
            BrigadeFriendlyVoting: voting.BrigadeFriendlyVoting,
            TeamFriendlyVoting: voting.TeamFriendlyVoting,
            VotesQuantity: voting.VotesQuantity,
            BannedTeams: [.. voting.BannedTeams],
            Candidates: [.. voting.Candidates
                .Select(candidate => new ParticipantVotingFormModel.CandidateModel(
                    Identifer: candidate.Identifier,
                    Name: candidate.Name,
                    Brigades: [.. candidate.Brigades],
                    Teams: [.. candidate.Teams]))]);

        var viewSelectedCandidates = new ParticipantVotingFormModel.CandidateModel?[viewVoting.VotesQuantity];

        var result = await _localStorage.GetAsync<string>(GetLocalStorageKey());
        return new ParticipantVotingFormModel(
            Voting: viewVoting,
            Form: new ParticipantVotingFormModel.FormModel(
                SelectedTeam: null,
                SelectedParticipant: null,
                SelectedCandidates: viewSelectedCandidates,
                EnteredNote: null),
            IsFormDisabled: result.Success,
            IsFormValid: false);
    }

    private async Task ProcessForm()
    {
        await _formSelector!.ValidateAsync();
        if (!ViewModel.IsFormValid)
        {
            return;
        }

        _databaseContext.ParticipantVotes.AddRange(ViewModel.Form.SelectedCandidates.Select(candidate => new ParticipantVote
        {
            ParticipantId = ViewModel.Form.SelectedParticipant!.Identifier,
            CandidateId = candidate!.Identifer,
            Note = ViewModel.Form.EnteredNote,
            ReceivedAt = DateTimeOffset.Now
        }));

        await _databaseContext.SaveChangesAsync();
        await _localStorage.SetAsync(GetLocalStorageKey(), DateTime.UtcNow.ToString(new CultureInfo("ru-RU")));

        _navigationManager.NavigateTo($"/grading/votings/{VotingId}/participant-voting", true);
    }

    private Task<IEnumerable<ParticipantVotingFormModel.TeamModel>> SearchTeam(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Task.FromResult(Enumerable.Empty<ParticipantVotingFormModel.TeamModel>());
        }

        return Task.FromResult<IEnumerable<ParticipantVotingFormModel.TeamModel>>([.. _dataConfiguration.Value.Teams
            .Where(team => !ViewModel.Voting!.BannedTeams.Contains(team.Identifier))
            .Where(team => team.Name.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Select(team => new ParticipantVotingFormModel.TeamModel(
                Identifier: team.Identifier,
                Name: team.Name))]);
    }

    private Task<IEnumerable<ParticipantVotingFormModel.ParticipantModel>> SearchParticipant(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Task.FromResult(Enumerable.Empty<ParticipantVotingFormModel.ParticipantModel>());
        }
        if (ViewModel.Form.SelectedTeam == null)
        {
            return Task.FromResult(Enumerable.Empty<ParticipantVotingFormModel.ParticipantModel>());
        }

        return Task.FromResult<IEnumerable<ParticipantVotingFormModel.ParticipantModel>>([.. _dataConfiguration.Value.Teams
            .Where(team => team.Identifier == ViewModel.Form.SelectedTeam.Identifier)
            .SelectMany(team => _dataConfiguration.Value.Participants
                .Where(participant => participant.Team == team.Identifier))
            .Where(participant => participant.Name.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Select(participant => new ParticipantVotingFormModel.ParticipantModel(
                Identifier: participant.Identifier,
                Name: participant.Name,
                Brigade: participant.Brigade,
                Team: participant.Team))]);
    }

    private string? ValidateSelectedCandidate(int index, ParticipantVotingFormModel.CandidateModel value)
    {
        if (value == null)
        {
            return null;
        }

        var friendlyBrigadeSelected =
            !string.IsNullOrWhiteSpace(ViewModel.Form.SelectedParticipant!.Brigade) &&
            value.Brigades.Contains(ViewModel.Form.SelectedParticipant!.Brigade);

        if (!ViewModel.Voting!.BrigadeFriendlyVoting && friendlyBrigadeSelected)
        {
            return "Нельзя голосовать за кандидата, связанного с твоим отрядом";
        }
        if (!ViewModel.Voting!.TeamFriendlyVoting && value.Teams.Contains(ViewModel.Form.SelectedParticipant!.Team))
        {
            return "Нельзя голосовать за кандидата, связанного с твоей командой";
        }
        if (index > 0 && ViewModel.Form.SelectedCandidates.Take(index).Contains(value))
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
        if (ViewModel.Form.SelectedParticipant == null)
        {
            return true;
        }
        if (index == 0)
        {
            return false;
        }

        return ViewModel.Form.SelectedCandidates[index - 1] == null;
    }

    private string GetLocalStorageKey() => $"AudienceVoting-{VotingId}";
}
