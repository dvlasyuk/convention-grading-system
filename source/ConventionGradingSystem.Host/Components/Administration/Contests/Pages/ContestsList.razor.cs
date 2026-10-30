using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.Host.Components.Administration.Contests.Components;
using ConventionGradingSystem.Host.Components.Administration.Contests.Models;
using ConventionGradingSystem.Host.Components.Shared.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using MudBlazor;

namespace ConventionGradingSystem.Host.Components.Administration.Contests.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class ContestsList
{
    private readonly IDbContextFactory<DatabaseContext> _databaseContextFactory;
    private readonly IOptions<DataConfiguration> _dataConfiguration;
    private readonly IDialogService _dialogService;

    public ContestsList(
        IDbContextFactory<DatabaseContext> databaseContextFactory,
        IOptions<DataConfiguration> dataConfiguration,
        IDialogService dialogService)
    {
        _databaseContextFactory = databaseContextFactory;
        _dataConfiguration = dataConfiguration;
        _dialogService = dialogService;
    }

    protected override async Task<ContentState<ContestsListModel>> LoadContent()
    {
        using var databaseContext = _databaseContextFactory.CreateDbContext();

        var expertFeedbacks = await databaseContext.ExpertFeedbacks.ToArrayAsync();
        var participantFeedbacks = await databaseContext.ParticipantFeedbacks.ToArrayAsync();

        var teams = _dataConfiguration.Value.Teams.ToDictionary(team => team.Identifier);
        var viewContests = _dataConfiguration.Value.Contests
            .OrderBy(contest => contest.Identifier)
            .Select((contest, index) => new ContestsListModel.ContestModel(
                Number: index + 1,
                Identifier: contest.Identifier,
                Name: contest.Name,
                ExpertGrading: contest.ExpertCriterions.Count > 0,
                ParticipantGrading: contest.ParticipantCriterions.Count > 0,
                RegisteredGrading: contest.RegisteredGrading,
                FriendlyGrading: contest.FriendlyGrading,
                AttendanceControl: contest.AttendanceControl,
                BannedTeams: [.. contest.BannedTeams
                    .Select(identifier => teams[identifier])
                    .Select(team => team.Name)],
                EventsQuantity: contest.Events.Count,
                ExpertFeedbaksQuantity: contest.Events.Sum(@event =>
                    expertFeedbacks.Count(feedback => feedback.EventId == @event.Identifier)),
                ParticiapntFeedbaksQuantity: contest.Events.Sum(@event =>
                    participantFeedbacks.Count(feedback => feedback.EventId == @event.Identifier))))
            .ToArray();

        return new ContestsListModel(Contests: viewContests, SearchFilter: null);
    }

    private void OpenContestCard(ContestsListModel.ContestModel contest)
    {
        var dialogViewModel = new ContestCardModel(
            ContestName: contest.Name,
            ExpertGrading: contest.ExpertGrading,
            ParticipantGrading: contest.ParticipantGrading,
            RegisteredGrading: contest.RegisteredGrading,
            FriendlyGrading: contest.FriendlyGrading,
            AttendanceControl: contest.AttendanceControl,
            BannedTeams: contest.BannedTeams);

        var dialogOptions = new DialogOptions { CloseButton = true };
        var dialogParameters = new DialogParameters<ContestCard>
        {
            { dialog => dialog.ViewModel, dialogViewModel }
        };

        _dialogService.ShowAsync<ContestCard>(dialogParameters, dialogOptions);
    }

    private bool FilterRecords(ContestsListModel.ContestModel contest)
    {
        return string.IsNullOrWhiteSpace(ViewModel.SearchFilter) ||
            contest.Name.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            contest.Identifier.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase);
    }

    private static object SelectNameField(ContestsListModel.ContestModel contest) => contest.Name;
    private static object SelectEventsField(ContestsListModel.ContestModel contest) => contest.EventsQuantity;
    private static object SelectExpertField(ContestsListModel.ContestModel contest) => contest.ExpertFeedbaksQuantity;
    private static object SelectParticipantField(ContestsListModel.ContestModel contest) => contest.ParticiapntFeedbaksQuantity;
}
