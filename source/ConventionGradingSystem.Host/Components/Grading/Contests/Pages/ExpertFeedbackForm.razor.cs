using System.Globalization;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.Host.Components.Grading.Contests.Models;
using ConventionGradingSystem.Host.Components.Shared.Models;
using ConventionGradingSystem.Host.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using MudBlazor;

namespace ConventionGradingSystem.Host.Components.Grading.Contests.Pages;

public partial class ExpertFeedbackForm
{
    private readonly IDbContextFactory<DatabaseContext> _databaseContextFactory;
    private readonly IOptions<DataConfiguration> _dataConfiguration;
    private readonly NavigationManager _navigationManager;
    private readonly ProtectedLocalStorage _localStorage;

    private MudForm? _formSelector;

    [Parameter]
    public string? EventId { get; set; }

    public ExpertFeedbackForm(
        IDbContextFactory<DatabaseContext> databaseContextFactory,
        IOptions<DataConfiguration> dataConfiguration,
        NavigationManager navigationManager,
        ProtectedLocalStorage localStorage)
    {
        _databaseContextFactory = databaseContextFactory;
        _dataConfiguration = dataConfiguration;
        _navigationManager = navigationManager;
        _localStorage = localStorage;
    }

    protected override async Task<ContentState<ExpertFeedbackFormModel>> LoadContent()
    {
        var contestEvent = _dataConfiguration.Value.Contests
            .SelectMany(@event => @event.Events)
            .FirstOrDefault(@event => @event.Identifier == EventId);

        if (contestEvent == null)
        {
            return Shared.Models.LoadingError.EventNotFound;
        }

        var contest = _dataConfiguration.Value.Contests.First(item => item.Events.Contains(contestEvent));
        var viewEvent = new ExpertFeedbackFormModel.EventModel(
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

        var viewSelectedGrades = new Dictionary<string, int>();
        foreach (var criterion in viewEvent.Criterions)
        {
            viewSelectedGrades.Add(criterion.Identifier, criterion.MaximalGrade);
        }

        var result = await _localStorage.GetAsync<string>(GetLocalStorageKey());
        return new ExpertFeedbackFormModel(
            Event: viewEvent,
            Form: new ExpertFeedbackFormModel.FormModel(
                SelectedExpert: null,
                SelectedGrades: viewSelectedGrades,
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

        var feedback = new DataAccess.Database.Entities.ExpertFeedback
        {
            EventId = EventId!,
            ExpertId = ViewModel.Form.SelectedExpert!.Identifier,
            Note = ViewModel.Form.EnteredNote,
            ReceivedAt = DateTimeOffset.Now
        };

        foreach (var criterionId in ViewModel.Form.SelectedGrades.Keys)
        {
            feedback.Grades.Add(new DataAccess.Database.Entities.ExpertGrade
            {
                CriterionId = criterionId,
                GradeValue = ViewModel.Form.SelectedGrades[criterionId]
            });
        }

        using var databaseContext = _databaseContextFactory.CreateDbContext();
        databaseContext.ExpertFeedbacks.Add(feedback);

        await databaseContext.SaveChangesAsync();
        await _localStorage.SetAsync(
            key: GetLocalStorageKey(),
            value: DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));

        _navigationManager.NavigateTo(
            uri: NavigationHelper.Grading.ExpertFeedbackForm(EventId!),
            forceLoad: true);
    }

    private Task<IEnumerable<ExpertFeedbackFormModel.ExpertModel>> SearchExpert(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Task.FromResult(Enumerable.Empty<ExpertFeedbackFormModel.ExpertModel>());
        }

        return Task.FromResult<IEnumerable<ExpertFeedbackFormModel.ExpertModel>>([.. _dataConfiguration.Value.Experts
            .Where(expert => expert.Name.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Select(expert => new ExpertFeedbackFormModel.ExpertModel(
                Identifier: expert.Identifier,
                Name: expert.Name))]);
    }

    private static string[] GetSliderLabels(ExpertFeedbackFormModel.CriterionModel criterion) => [.. Enumerable
        .Range(criterion.MinimalGrade, criterion.MaximalGrade)
        .Select(label => label.ToString(CultureInfo.CurrentCulture))];

    private string GetLocalStorageKey() => $"ExpertFeedback-{EventId}";
}
