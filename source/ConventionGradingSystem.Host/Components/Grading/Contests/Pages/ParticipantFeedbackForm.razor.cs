using System.Globalization;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.Host.Components.Grading.Contests.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;

using MudBlazor;

namespace ConventionGradingSystem.Host.Components.Grading.Contests.Pages;

public partial class ParticipantFeedbackForm
{
    private readonly DatabaseContext _databaseContext;
    private readonly IOptions<ApplicationConfiguration> _configuration;
    private readonly NavigationManager _navigationManager;
    private readonly ProtectedLocalStorage _localStorage;

    private readonly ParticipantFeedbackFormModel _viewModel = new();
    private MudForm? _formSelector;
    private MudAutocomplete<ParticipantFeedbackFormModel.ParticipantModel>? _participantSelector;

    [Parameter]
    public string? EventId { get; set; }

    public ParticipantFeedbackForm(
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
        var contestEvent = _configuration.Value.Contests
            .SelectMany(@event => @event.Events)
            .FirstOrDefault(@event => @event.Identifier == EventId);

        if (contestEvent == null)
        {
            return;
        }

        var result = await _localStorage.GetAsync<string>(GetLocalStorageKey());
        _viewModel.IsFormDisabled = result.Success;

        var contest = _configuration.Value.Contests.First(item => item.Events.Contains(contestEvent));
        _viewModel.Event = new ParticipantFeedbackFormModel.EventModel(
            EventName: contestEvent.Name,
            ContestName: contest.Name,
            IsGraded: contest.ParticipantCriterions.Count > 0,
            RegisteredGrading: contest.RegisteredGrading,
            FriendlyGrading: contest.FriendlyGrading,
            BannedTeams: contest.BannedTeams.ToList(),
            Brigades: contestEvent.Brigades.ToList(),
            Participants: contestEvent.Participants.ToList(),
            Criterions: [.. contest.ParticipantCriterions
                .Select(criterion => new ParticipantFeedbackFormModel.CriterionModel(
                    Identifier: criterion.Identifier,
                    Name: criterion.Name,
                    Description: criterion.Description,
                    MinimalGrade: criterion.MinimalGrade,
                    MaximalGrade: criterion.MaximalGrade))]);

        foreach (var criterion in _viewModel.Event.Criterions)
        {
            _viewModel.SelectedGrades.Add(criterion.Identifier, criterion.MaximalGrade);
        }
    }

    private async Task ProcessForm()
    {
        await _formSelector!.ValidateAsync();
        if (!_viewModel.IsFormValid)
        {
            return;
        }

        var feedback = new DataAccess.Database.Entities.ParticipantFeedback
        {
            EventId = EventId!,
            ParticipantId = _viewModel.SelectedParticipant!.Identifier,
            Note = _viewModel.EnteredNote,
            ReceivedAt = DateTimeOffset.Now
        };

        foreach (var criterionId in _viewModel.SelectedGrades.Keys)
        {
            feedback.Grades.Add(new DataAccess.Database.Entities.ParticipantGrade
            {
                CriterionId = criterionId,
                GradeValue = _viewModel.SelectedGrades[criterionId]
            });
        }

        _databaseContext.ParticipantFeedbacks.Add(feedback);

        await _databaseContext.SaveChangesAsync();
        await _localStorage.SetAsync(
            key: GetLocalStorageKey(),
            value: DateTime.UtcNow.ToString(new CultureInfo("ru-RU")));

        _navigationManager.NavigateTo($"/grading/events/{EventId}/participant-feedback", true);
    }

    private Task<IEnumerable<ParticipantFeedbackFormModel.TeamModel>> SearchTeam(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Task.FromResult(Enumerable.Empty<ParticipantFeedbackFormModel.TeamModel>());
        }

        return Task.FromResult<IEnumerable<ParticipantFeedbackFormModel.TeamModel>>([.. _configuration.Value.Teams
            .Where(team => !_viewModel.Event!.BannedTeams.Contains(team.Identifier))
            .Where(team => _configuration.Value.Participants
                .Where(participant => participant.Team == team.Identifier)
                .Where(participant =>
                    !_viewModel.Event!.RegisteredGrading ||
                    _viewModel.Event!.Participants.Contains(participant.Identifier))
                .Where(participant =>
                    _viewModel.Event!.FriendlyGrading ||
                    string.IsNullOrWhiteSpace(participant.Brigade) ||
                    !_viewModel.Event!.Brigades.Contains(participant.Brigade))
                .Any())
            .Where(team => team.Name.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Select(team => new ParticipantFeedbackFormModel.TeamModel(
                Identifier: team.Identifier,
                Name: team.Name))]);
    }

    private Task<IEnumerable<ParticipantFeedbackFormModel.ParticipantModel>> SearchParticipant(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Task.FromResult(Enumerable.Empty<ParticipantFeedbackFormModel.ParticipantModel>());
        }
        if (_viewModel.SelectedTeam == null)
        {
            return Task.FromResult(Enumerable.Empty<ParticipantFeedbackFormModel.ParticipantModel>());
        }

        return Task.FromResult<IEnumerable<ParticipantFeedbackFormModel.ParticipantModel>>([.. _configuration.Value.Teams
            .Where(team => team.Identifier == _viewModel.SelectedTeam.Identifier)
            .SelectMany(team => _configuration.Value.Participants
                .Where(participant => participant.Team == team.Identifier))
            .Where(participant =>
                !_viewModel.Event!.RegisteredGrading ||
                _viewModel.Event!.Participants.Contains(participant.Identifier))
            .Where(participant =>
                _viewModel.Event!.FriendlyGrading ||
                string.IsNullOrWhiteSpace(participant.Brigade) ||
                !_viewModel.Event!.Brigades.Contains(participant.Brigade))
            .Where(participant => participant.Name.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Select(participant => new ParticipantFeedbackFormModel.ParticipantModel(
                Identifier: participant.Identifier,
                Name: participant.Name))]);
    }

    private async Task OnSelectedTeamChange()
    {
        await _participantSelector!.ResetAsync();
    }

    private static string[] GetSliderLabels(ParticipantFeedbackFormModel.CriterionModel criterion) => [.. Enumerable
        .Range(criterion.MinimalGrade, criterion.MaximalGrade)
        .Select(label => label.ToString(new CultureInfo("ru-RU")))];

    private string GetLocalStorageKey() => $"ParticipantFeedback-{EventId}";
}
