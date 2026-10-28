using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.Host.Components.Administration.Contests.Models;
using ConventionGradingSystem.Host.Components.Shared.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Administration.Contests.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class ContestsList
{
    private readonly IDbContextFactory<DatabaseContext> _databaseContextFactory;
    private readonly IOptions<DataConfiguration> _dataConfiguration;

    public ContestsList(
        IDbContextFactory<DatabaseContext> databaseContextFactory,
        IOptions<DataConfiguration> dataConfiguration)
    {
        _databaseContextFactory = databaseContextFactory;
        _dataConfiguration = dataConfiguration;
    }

    protected override async Task<ContentState<ContestsListModel>> LoadContent()
    {
        using var databaseContext = _databaseContextFactory.CreateDbContext();

        var expertFeedbacks = await databaseContext.ExpertFeedbacks.ToListAsync();
        var participantFeedbacks = await databaseContext.ParticipantFeedbacks.ToListAsync();

        var viewContests = _dataConfiguration.Value.Contests
            .OrderBy(contest => contest.Identifier)
            .Select((contest, index) => new ContestsListModel.ContestModel(
                Number: index + 1,
                Identifier: contest.Identifier,
                Name: contest.Name,
                EventsQuantity: contest.Events.Count,
                ExpertFeedbaksQuantity: contest.Events.Sum(@event =>
                    expertFeedbacks.Count(feedback => feedback.EventId == @event.Identifier)),
                ParticiapntFeedbaksQuantity: contest.Events.Sum(@event =>
                    participantFeedbacks.Count(feedback => feedback.EventId == @event.Identifier))))
            .ToArray();

        return new ContestsListModel(Contests: viewContests, SearchFilter: null);
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
