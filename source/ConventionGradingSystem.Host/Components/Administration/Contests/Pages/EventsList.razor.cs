using System.Diagnostics.CodeAnalysis;

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
public partial class EventsList
{
    private readonly IDbContextFactory<DatabaseContext> _databaseContextFactory;
    private readonly IOptions<DataConfiguration> _dataConfiguration;
    private readonly IDialogService _dialogService;

    [Parameter]
    public string? ContestId { get; set; }

    public EventsList(
        IDbContextFactory<DatabaseContext> databaseContextFactory,
        IOptions<DataConfiguration> dataConfiguration,
        IDialogService dialogService)
    {
        _databaseContextFactory = databaseContextFactory;
        _dataConfiguration = dataConfiguration;
        _dialogService = dialogService;
    }

    protected override async Task<ContentState<EventsListModel>> LoadContent()
    {
        var contest = _dataConfiguration.Value.Contests.FirstOrDefault(item => item.Identifier == ContestId);
        if (contest == null)
        {
            return Shared.Models.LoadingError.ContestNotFound;
        }

        var teams = _dataConfiguration.Value.Teams.ToDictionary(team => team.Identifier);
        var viewContest = new EventsListModel.ContestModel(
            Name: contest.Name,
            ExpertGrading: contest.ExpertCriterions.Count > 0,
            ParticipantGrading: contest.ParticipantCriterions.Count > 0,
            RegisteredGrading: contest.RegisteredGrading,
            FriendlyGrading: contest.FriendlyGrading,
            AttendanceControl: contest.AttendanceControl,
            BannedTeams: [.. contest.BannedTeams
                .Select(identifier => teams[identifier])
                .Select(team => team.Name)]);

        using var databaseContext = _databaseContextFactory.CreateDbContext();

        var eventIds = contest.Events
            .Select(item => item.Identifier)
            .ToList();

        var expertFeedbacks = await databaseContext.ExpertFeedbacks
            .Where(item => eventIds.Contains(item.EventId))
            .Include(item => item.Grades)
            .ToArrayAsync();

        var participantFeedbacks = await databaseContext.ParticipantFeedbacks
            .Where(item => eventIds.Contains(item.EventId))
            .Include(item => item.Grades)
            .ToArrayAsync();

        var brigades = _dataConfiguration.Value.Brigades.ToDictionary(brigade => brigade.Identifier);
        var viewEvents = contest.Events
            .OrderBy(contestEvent => contestEvent.Identifier)
            .Select((contestEvent, index) => new EventsListModel.EventModel(
                Number: index + 1,
                Identifier: contestEvent.Identifier,
                Name: contestEvent.Name,
                ParticipantsRegistered: contestEvent.Participants.Count > 0,
                Brigades: [.. contestEvent.Brigades
                    .Select(identifier => brigades[identifier])
                    .Select(brigade => brigade.Name)],
                ExprertFeedbacksQuantity: expertFeedbacks
                    .Count(feedback => feedback.EventId == contestEvent.Identifier),
                ParticipantFeedbacksQuantity: participantFeedbacks
                    .Count(feedback => feedback.EventId == contestEvent.Identifier),
                ExpertGrade: expertFeedbacks
                    .Where(feedback => feedback.EventId == contestEvent.Identifier)
                    .SelectMany(feedback => feedback.Grades)
                    .GroupBy(grade => grade.CriterionId)
                    .Sum(group => group.Sum(grade => grade.GradeValue) / (float)group.Count()),
                ParticipantGrade: participantFeedbacks
                    .Where(feedback => feedback.EventId == contestEvent.Identifier)
                    .SelectMany(feedback => feedback.Grades)
                    .GroupBy(grade => grade.CriterionId)
                    .Sum(group => group.Sum(grade => grade.GradeValue) / (float)group.Count()),
                TotalGrade:
                    expertFeedbacks
                        .Where(feedback => feedback.EventId == contestEvent.Identifier)
                        .SelectMany(feedback => feedback.Grades)
                        .GroupBy(grade => grade.CriterionId)
                        .Sum(group => group.Sum(grade => grade.GradeValue) / (float)group.Count()) +
                    participantFeedbacks
                        .Where(feedback => feedback.EventId == contestEvent.Identifier)
                        .SelectMany(feedback => feedback.Grades)
                        .GroupBy(grade => grade.CriterionId)
                        .Sum(group => group.Sum(grade => grade.GradeValue) / (float)group.Count())))
            .ToArray();

        return new EventsListModel(
            Contest: viewContest,
            Events: viewEvents,
            SearchFilter: null);
    }

    private void OpenContestCard()
    {
        var dialogViewModel = new ContestCardModel(
            ContestName: ViewModel.Contest.Name,
            ExpertGrading: ViewModel.Contest.ExpertGrading,
            ParticipantGrading: ViewModel.Contest.ParticipantGrading,
            RegisteredGrading: ViewModel.Contest.RegisteredGrading,
            FriendlyGrading: ViewModel.Contest.FriendlyGrading,
            AttendanceControl: ViewModel.Contest.AttendanceControl,
            BannedTeams: ViewModel.Contest.BannedTeams);

        var dialogOptions = new DialogOptions { CloseButton = true };
        var dialogParameters = new DialogParameters<ContestCard>
        {
            { dialog => dialog.ViewModel, dialogViewModel }
        };

        _dialogService.ShowAsync<ContestCard>(dialogParameters, dialogOptions);
    }

    private void OpenEventCard(EventsListModel.EventModel contestEvent)
    {
        var dialogViewModel = new EventCardModel(
            EventName: contestEvent.Name,
            ContestName: ViewModel.Contest.Name,
            ParticipantsRegistered: contestEvent.ParticipantsRegistered,
            Brigades: contestEvent.Brigades);

        var dialogOptions = new DialogOptions { CloseButton = true };
        var dialogParameters = new DialogParameters<EventCard>
        {
            { dialog => dialog.ViewModel, dialogViewModel }
        };

        _dialogService.ShowAsync<EventCard>(dialogParameters, dialogOptions);
    }

    private bool FilterRecords(EventsListModel.EventModel contestEvent)
    {
        return string.IsNullOrWhiteSpace(ViewModel.SearchFilter) ||
            contestEvent.Name.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            contestEvent.Identifier.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase);
    }

    private static object SelectNameField(EventsListModel.EventModel contestEvent) => contestEvent.Name;
    private static object SelectExpertFeedbacksField(EventsListModel.EventModel contestEvent) => contestEvent.ExprertFeedbacksQuantity;
    private static object SelectParticipantFeedbacksField(EventsListModel.EventModel contestEvent) => contestEvent.ParticipantFeedbacksQuantity;
    private static object SelectExpertGradeField(EventsListModel.EventModel contestEvent) => contestEvent.ExpertGrade;
    private static object SelectParticipantGradeField(EventsListModel.EventModel contestEvent) => contestEvent.ParticipantGrade;
    private static object SelectTotalGradeField(EventsListModel.EventModel contestEvent) => contestEvent.TotalGrade;
}
