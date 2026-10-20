using System.Globalization;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.Host.Navigation;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;

using MudBlazor;

namespace ConventionGradingSystem.Host.Components.Grading.Contests.Pages;

public partial class ExpertFeedbackForm
{
    private readonly DatabaseContext _databaseContext;
    private readonly IOptions<ApplicationConfiguration> _configuration;
    private readonly NavigationManager _navigationManager;
    private readonly ProtectedLocalStorage _localStorage;

    private EventDetails? _eventDetails;
    private bool _isFormDisabled;
    private bool _isFormValid;

    private Expert? _selectedExpert;
    private readonly Dictionary<string, int> _selectedGrades = [];
    private string? _enteredNote;

    private MudForm? _formSelector;

    [Parameter]
    public string? EventId { get; set; }

    public ExpertFeedbackForm(
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
            IsGraded: contest.ExpertCriterions.Count > 0,
            Criterions: [.. contest.ExpertCriterions
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

        var feedback = new DataAccess.Database.Entities.ExpertFeedback
        {
            EventId = EventId!,
            ExpertId = _selectedExpert!.Identifier,
            Note = _enteredNote,
            ReceivedAt = DateTimeOffset.Now
        };

        foreach (var criterionId in _selectedGrades.Keys)
        {
            feedback.Grades.Add(new DataAccess.Database.Entities.ExpertGrade
            {
                CriterionId = criterionId,
                GradeValue = _selectedGrades[criterionId]
            });
        }

        _databaseContext.ExpertFeedbacks.Add(feedback);

        await _databaseContext.SaveChangesAsync();
        await _localStorage.SetAsync(
            key: GetLocalStorageKey(),
            value: DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));

        _navigationManager.NavigateTo(
            uri: NavigationHelper.Grading.GetExpertFeedbackFormLink(EventId!),
            forceLoad: true);
    }

    private Task<IEnumerable<Expert>> SearchExpert(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Task.FromResult(Enumerable.Empty<Expert>());
        }

        return Task.FromResult<IEnumerable<Expert>>([.. _configuration.Value.Experts
            .Where(expert => expert.Name.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Select(expert => new Expert(
                Identifier: expert.Identifier,
                Name: expert.Name))]);
    }

    private static string[] GetSliderLabels(GradeCriterion criterion) => [.. Enumerable
        .Range(criterion.MinimalGrade, criterion.MaximalGrade)
        .Select(label => label.ToString(CultureInfo.CurrentCulture))];

    private string GetLocalStorageKey() => $"ExpertFeedback-{EventId}";

    private sealed record EventDetails(
        string EventName,
        string ContestName,
        bool IsGraded,
        IReadOnlyList<GradeCriterion> Criterions);

    private sealed record Expert(string Identifier, string Name)
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
