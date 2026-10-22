using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.Host.Components.Administration.Contests.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Administration.Contests.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class ContestsList
{
    private readonly DatabaseContext _databaseContext;
    private readonly IOptions<ApplicationConfiguration> _configuration;

    private readonly ContestsListModel _viewModel = new();

    public ContestsList(
        DatabaseContext databaseContext,
        IOptions<ApplicationConfiguration> configuration)
    {
        _databaseContext = databaseContext;
        _configuration = configuration;
    }

    protected override async Task OnInitializedAsync()
    {
        var expertFeedbacks = await _databaseContext.ExpertFeedbacks.ToListAsync();
        var participantFeedbacks = await _databaseContext.ParticipantFeedbacks.ToListAsync();

        _viewModel.Contests = [.. _configuration.Value.Contests
            .OrderBy(contest => contest.Identifier)
            .Select((contest, index) => new ContestsListModel.ContestModel(
                Number: index + 1,
                Identifier: contest.Identifier,
                Name: contest.Name,
                EventsQuantity: contest.Events.Count,
                ExpertFeedbaksQuantity: contest.Events.Sum(@event =>
                    expertFeedbacks.Count(feedback => feedback.EventId == @event.Identifier)),
                ParticiapntFeedbaksQuantity: contest.Events.Sum(@event =>
                    participantFeedbacks.Count(feedback => feedback.EventId == @event.Identifier)),
                EventsListLink: $"/administration/contests/{contest.Identifier}/events"))];
    }

    private bool FilterRecords(ContestsListModel.ContestModel contest)
    {
        return string.IsNullOrWhiteSpace(_viewModel.SearchFilter) ||
            contest.Name.Contains(_viewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            contest.Identifier.Contains(_viewModel.SearchFilter, StringComparison.OrdinalIgnoreCase);
    }

    private static object SelectNameField(ContestsListModel.ContestModel contest) => contest.Name;
    private static object SelectEventsField(ContestsListModel.ContestModel contest) => contest.EventsQuantity;
    private static object SelectExpertField(ContestsListModel.ContestModel contest) => contest.ExpertFeedbaksQuantity;
    private static object SelectParticipantField(ContestsListModel.ContestModel contest) => contest.ParticiapntFeedbaksQuantity;
}
