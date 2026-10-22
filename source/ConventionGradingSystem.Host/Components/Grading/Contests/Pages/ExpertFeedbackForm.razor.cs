using System.Globalization;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.Host.Components.Grading.Contests.Models;

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

    private readonly ExpertFeedbackFormModel _viewModel = new();
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
        _viewModel.IsFormDisabled = result.Success;

        var contest = _configuration.Value.Contests.First(item => item.Events.Contains(contestEvent));
        _viewModel.Event = new ExpertFeedbackFormModel.EventModel(
            EventName: contestEvent.Name,
            ContestName: contest.Name,
            IsGraded: contest.ExpertCriterions.Count > 0,
            Criterions: [.. contest.ExpertCriterions
                .Select(criterion => new ExpertFeedbackFormModel.CriterionModel(
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

        var feedback = new DataAccess.Database.Entities.ExpertFeedback
        {
            EventId = EventId!,
            ExpertId = _viewModel.SelectedExpert!.Identifier,
            Note = _viewModel.EnteredNote,
            ReceivedAt = DateTimeOffset.Now
        };

        foreach (var criterionId in _viewModel.SelectedGrades.Keys)
        {
            feedback.Grades.Add(new DataAccess.Database.Entities.ExpertGrade
            {
                CriterionId = criterionId,
                GradeValue = _viewModel.SelectedGrades[criterionId]
            });
        }

        _databaseContext.ExpertFeedbacks.Add(feedback);

        await _databaseContext.SaveChangesAsync();
        await _localStorage.SetAsync(
            key: GetLocalStorageKey(),
            value: DateTime.UtcNow.ToString(new CultureInfo("ru-RU")));

        _navigationManager.NavigateTo($"/grading/events/{EventId}/expert-feedback", true);
    }

    private Task<IEnumerable<ExpertFeedbackFormModel.ExpertModel>> SearchExpert(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Task.FromResult(Enumerable.Empty<ExpertFeedbackFormModel.ExpertModel>());
        }

        return Task.FromResult<IEnumerable<ExpertFeedbackFormModel.ExpertModel>>([.. _configuration.Value.Experts
            .Where(expert => expert.Name.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Select(expert => new ExpertFeedbackFormModel.ExpertModel(
                Identifier: expert.Identifier,
                Name: expert.Name))]);
    }

    private static string[] GetSliderLabels(ExpertFeedbackFormModel.CriterionModel criterion) => [.. Enumerable
        .Range(criterion.MinimalGrade, criterion.MaximalGrade)
        .Select(label => label.ToString(new CultureInfo("ru-RU")))];

    private string GetLocalStorageKey() => $"ExpertFeedback-{EventId}";
}
