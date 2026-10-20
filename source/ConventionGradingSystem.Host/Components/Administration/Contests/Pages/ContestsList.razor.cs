using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Administration.Contests.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class ContestsList
{
    private readonly DatabaseContext _databaseContext;
    private readonly IOptions<ApplicationConfiguration> _configuration;

    private IEnumerable<Contest> _contests = [];
    private string? _searchFilter;

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

        _contests = [.. _configuration.Value.Contests
            .OrderBy(contest => contest.Identifier)
            .Select((contest, index) => new Contest(
                Number: index + 1,
                Identifier: contest.Identifier,
                Name: contest.Name,
                EventsQuantity: contest.Events.Count,
                ExpertFeedbaksQuantity: contest.Events.Sum(@event =>
                    expertFeedbacks.Count(feedback => feedback.EventId == @event.Identifier)),
                ParticiapntFeedbaksQuantity: contest.Events.Sum(@event =>
                    participantFeedbacks.Count(feedback => feedback.EventId == @event.Identifier))))];
    }

    private bool FilterRecords(Contest contest)
    {
        return string.IsNullOrWhiteSpace(_searchFilter) ||
            contest.Name.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase) ||
            contest.Identifier.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase);
    }

    private static object SelectNameField(Contest contest) => contest.Name;
    private static object SelectEventsField(Contest contest) => contest.EventsQuantity;
    private static object SelectExpertField(Contest contest) => contest.ExpertFeedbaksQuantity;
    private static object SelectParticipantField(Contest contest) => contest.ParticiapntFeedbaksQuantity;

    private sealed record Contest(
        int Number,
        string Identifier,
        string Name,
        int EventsQuantity,
        int ExpertFeedbaksQuantity,
        int ParticiapntFeedbaksQuantity);
}
