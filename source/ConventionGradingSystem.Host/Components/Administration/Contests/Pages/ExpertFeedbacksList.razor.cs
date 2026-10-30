using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.Host.Components.Administration.Contests.Components;
using ConventionGradingSystem.Host.Components.Administration.Contests.Models;
using ConventionGradingSystem.Host.Components.Shared.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using MudBlazor;

namespace ConventionGradingSystem.Host.Components.Administration.Contests.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class ExpertFeedbacksList
{
    private readonly IDbContextFactory<DatabaseContext> _databaseContextFactory;
    private readonly IOptions<DataConfiguration> _dataConfiguration;
    private readonly IDialogService _dialogService;

    [Parameter]
    public string? EventId { get; set; }

    public ExpertFeedbacksList(
        IDbContextFactory<DatabaseContext> databaseContextFactory,
        IOptions<DataConfiguration> dataConfiguration,
        IDialogService dialogService)
    {
        _databaseContextFactory = databaseContextFactory;
        _dataConfiguration = dataConfiguration;
        _dialogService = dialogService;
    }

    protected override async Task<ContentState<ExpertFeedbacksListModel>> LoadContent()
    {
        var contestEvent = _dataConfiguration.Value.Contests
            .SelectMany(item => item.Events)
            .FirstOrDefault(item => item.Identifier == EventId);

        if (contestEvent == null)
        {
            return Shared.Models.LoadingError.EventNotFound;
        }

        var contest = _dataConfiguration.Value.Contests.First(item => item.Events.Contains(contestEvent));
        var brigades = _dataConfiguration.Value.Brigades.ToDictionary(brigade => brigade.Identifier);
        var viewEvent = new ExpertFeedbacksListModel.EventModel(
            EventName: contestEvent.Name,
            ContestName: contest.Name,
            ExpertGrading: contest.ExpertCriterions.Count > 0,
            ParticipantsRegistered: contestEvent.Participants.Count > 0,
            Brigades: [.. @contestEvent.Brigades
                .Select(identifier => brigades[identifier])
                .Select(brigade => brigade.Name)],
            GradeCriterions: [.. contest.ExpertCriterions
                .Select(criterion => new ExpertFeedbacksListModel.CriterionModel(
                    Identifier: criterion.Identifier,
                    Name: criterion.Name))]);

        using var databaseContext = _databaseContextFactory.CreateDbContext();
        var feedbacks = await databaseContext.ExpertFeedbacks
            .Where(item => item.EventId == contestEvent.Identifier)
            .Include(item => item.Grades)
            .ToArrayAsync();

        var experts = _dataConfiguration.Value.Experts.ToDictionary(item => item.Identifier);
        var viewFeedbacks = feedbacks
            .OrderBy(feedback => feedback.Identifier)
            .Select((feedback, index) => new ExpertFeedbacksListModel.FeedbackModel(
                Number: index + 1,
                ExpertName: experts[feedback.ExpertId].Name,
                Grades: feedback.Grades.ToDictionary(
                    grade => grade.CriterionId,
                    grade => grade.GradeValue),
                Note: feedback.Note,
                ReceivedAt: feedback.ReceivedAt.ToString(CultureInfo.CurrentCulture)))
            .ToArray();

        return new ExpertFeedbacksListModel(
            Event: viewEvent,
            Feedbacks: viewFeedbacks,
            SearchFilter: null);
    }

    private void OpenEventCard()
    {
        var dialogViewModel = new EventCardModel(
            EventName: ViewModel.Event.EventName,
            ContestName: ViewModel.Event.ContestName,
            ParticipantsRegistered: ViewModel.Event.ParticipantsRegistered,
            Brigades: ViewModel.Event.Brigades);

        var dialogOptions = new DialogOptions { CloseButton = true };
        var dialogParameters = new DialogParameters<EventCard>
        {
            { dialog => dialog.ViewModel, dialogViewModel }
        };

        _dialogService.ShowAsync<EventCard>(dialogParameters, dialogOptions);
    }

    private bool FilterRecords(ExpertFeedbacksListModel.FeedbackModel feedback)
    {
        return string.IsNullOrWhiteSpace(ViewModel.SearchFilter) ||
            feedback.ExpertName.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            (feedback.Note?.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private static object SelectNameField(ExpertFeedbacksListModel.FeedbackModel feedback) => feedback.ExpertName;
    private static object SelectGradeField(ExpertFeedbacksListModel.FeedbackModel feedback, string gradeId) => feedback.Grades[gradeId];
}
