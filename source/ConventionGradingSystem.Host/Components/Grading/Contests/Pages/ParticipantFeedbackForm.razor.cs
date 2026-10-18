using System.Globalization;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;

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

    private EventDetails? _eventDetails;
    private bool _isFormDisabled;
    private bool _isFormValid;

    private Team? _selectedTeam;
    private Participant? _selectedParticipant;
    private readonly Dictionary<string, int> _selectedGrades = [];
    private string? _enteredNote;

    private MudForm? _formSelector;
    private MudAutocomplete<Participant>? _participantSelector;

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
        _isFormDisabled = result.Success;

        var contest = _configuration.Value.Contests.First(item => item.Events.Contains(contestEvent));
        _eventDetails = new EventDetails(
            EventName: contestEvent.Name,
            ContestName: contest.Name,
            IsGraded: contest.ParticipantCriterions.Count > 0,
            RegisteredGrading: contest.RegisteredGrading,
            FriendlyGrading: contest.FriendlyGrading,
            BannedTeams: contest.BannedTeams.ToList(),
            Brigades: contestEvent.Brigades.ToList(),
            Participants: contestEvent.Participants.ToList(),
            Criterions: [.. contest.ParticipantCriterions
                .Select(criterion => new GradeCriterion(
                    Identifier: criterion.Identifier,
                    Name: criterion.Name,
                    Description: criterion.Description,
                    MinimalGrade: criterion.MinimalGrade,
                    MaximalGrade: criterion.MaximalGrade))]);

        foreach (var criterion in _eventDetails.Criterions)
        {
            _selectedGrades.Add(criterion.Identifier, criterion.MaximalGrade);
        }
    }

    private async Task ProcessForm()
    {
        await _formSelector!.ValidateAsync();
        if (!_isFormValid)
        {
            return;
        }

        var feedback = new DataAccess.Database.Entities.ParticipantFeedback
        {
            EventId = EventId!,
            ParticipantId = _selectedParticipant!.Identifier,
            Note = _enteredNote,
            ReceivedAt = DateTimeOffset.Now
        };

        foreach (var criterionId in _selectedGrades.Keys)
        {
            feedback.Grades.Add(new DataAccess.Database.Entities.ParticipantGrade
            {
                CriterionId = criterionId,
                GradeValue = _selectedGrades[criterionId]
            });
        }

        _databaseContext.ParticipantFeedbacks.Add(feedback);

        await _databaseContext.SaveChangesAsync();
        await _localStorage.SetAsync(
            key: GetLocalStorageKey(),
            value: DateTime.UtcNow.ToString(new CultureInfo("ru-RU")));

        _navigationManager.NavigateTo($"/grading/events/{EventId}/participant-feedback", true);
    }

    private Task<IEnumerable<Team>> SearchTeam(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Task.FromResult(Enumerable.Empty<Team>());
        }

        return Task.FromResult<IEnumerable<Team>>([.. _configuration.Value.Teams
            .Where(team => !_eventDetails!.BannedTeams.Contains(team.Identifier))
            .Where(team => _configuration.Value.Participants
                .Where(participant => participant.Team == team.Identifier)
                .Where(participant =>
                    !_eventDetails!.RegisteredGrading ||
                    _eventDetails!.Participants.Contains(participant.Identifier))
                .Where(participant =>
                    _eventDetails!.FriendlyGrading ||
                    string.IsNullOrWhiteSpace(participant.Brigade) ||
                    !_eventDetails!.Brigades.Contains(participant.Brigade))
                .Any())
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
            .Where(participant =>
                !_eventDetails!.RegisteredGrading ||
                _eventDetails!.Participants.Contains(participant.Identifier))
            .Where(participant =>
                _eventDetails!.FriendlyGrading ||
                string.IsNullOrWhiteSpace(participant.Brigade) ||
                !_eventDetails!.Brigades.Contains(participant.Brigade))
            .Where(participant => participant.Name.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Select(participant => new Participant(
                Identifier: participant.Identifier,
                Name: participant.Name))]);
    }

    private async Task OnSelectedTeamChange()
    {
        await _participantSelector!.ResetAsync();
    }

    private static string[] GetSliderLabels(GradeCriterion criterion) => [.. Enumerable
        .Range(criterion.MinimalGrade, criterion.MaximalGrade)
        .Select(label => label.ToString(new CultureInfo("ru-RU")))];

    private string GetLocalStorageKey() => $"ParticipantFeedback-{EventId}";

    private sealed record EventDetails(
        string EventName,
        string ContestName,
        bool IsGraded,
        bool RegisteredGrading,
        bool FriendlyGrading,
        IReadOnlyCollection<string> BannedTeams,
        IReadOnlyCollection<string> Brigades,
        IReadOnlyCollection<string> Participants,
        IReadOnlyList<GradeCriterion> Criterions);

    private sealed record Team(string Identifier, string Name)
    {
        public override string ToString() => Name;
    }

    private sealed record Participant(string Identifier, string Name)
    {
        public override string ToString() => Name;
    }

    private sealed record GradeCriterion(
        string Identifier,
        string Name,
        string Description,
        int MinimalGrade,
        int MaximalGrade);
}
