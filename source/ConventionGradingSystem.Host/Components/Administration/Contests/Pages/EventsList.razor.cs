using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Administration.Contests.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class EventsList
{
    private readonly DatabaseContext _databaseContext;
    private readonly IOptions<ApplicationConfiguration> _configuration;

    private Contest? _contest;
    private IEnumerable<ContestEvent> _contestEvents = [];
    private string? _searchFilter;

    [Parameter]
    public string? ContestId { get; set; }

    public EventsList(
        DatabaseContext databaseContext,
        IOptions<ApplicationConfiguration> configuration)
    {
        _databaseContext = databaseContext;
        _configuration = configuration;
    }

    protected override async Task OnInitializedAsync()
    {
        var contest = _configuration.Value.Contests.FirstOrDefault(item => item.Identifier == ContestId);
        if (contest == null)
        {
            return;
        }

        var eventIds = contest.Events
            .Select(item => item.Identifier)
            .ToList();

        var expertFeedbacks = await _databaseContext.ExpertFeedbacks
            .Where(item => eventIds.Contains(item.EventId))
            .Include(item => item.Grades)
            .ToListAsync();

        var participantFeedbacks = await _databaseContext.ParticipantFeedbacks
            .Where(item => eventIds.Contains(item.EventId))
            .Include(item => item.Grades)
            .ToListAsync();

        _contest = new Contest(
            Name: contest.Name,
            RegisteredGrading: contest.RegisteredGrading,
            FriendlyGrading: contest.FriendlyGrading,
            AttendanceControl: contest.AttendanceControl,
            ExpertCriterionsQuantity: contest.ExpertCriterions.Count,
            ParticipantCriterionsQuantity: contest.ParticipantCriterions.Count,
            EventsQuantity: contest.Events.Count,
            ExpertFeedbacksQuantity: expertFeedbacks.Count,
            ParticipantFeedbacksQuantity: participantFeedbacks.Count);

        var brigades = _configuration.Value.Brigades.ToDictionary(brigade => brigade.Identifier);
        _contestEvents = [.. contest.Events
            .OrderBy(@event => @event.Identifier)
            .Select((@event, index) => new ContestEvent(
                Number: index + 1,
                Identifier: @event.Identifier,
                Name: @event.Name,
                Brigades: @event.Brigades
                    .Select(identifier => brigades[identifier])
                    .Select(brigade => brigade.Name)
                    .ToList(),
                ExprertFeedbacksQuantity: expertFeedbacks
                    .Count(feedback => feedback.EventId == @event.Identifier),
                ParticipantFeedbacksQuantity: participantFeedbacks
                    .Count(feedback => feedback.EventId == @event.Identifier),
                ExpertGrade: expertFeedbacks
                    .Where(feedback => feedback.EventId == @event.Identifier)
                    .SelectMany(feedback => feedback.Grades)
                    .GroupBy(grade => grade.CriterionId)
                    .Sum(group => group.Sum(grade => grade.GradeValue) / (float)group.Count()),
                ParticipantGrade: participantFeedbacks
                    .Where(feedback => feedback.EventId == @event.Identifier)
                    .SelectMany(feedback => feedback.Grades)
                    .GroupBy(grade => grade.CriterionId)
                    .Sum(group => group.Sum(grade => grade.GradeValue) / (float)group.Count()),
                TotalGrade:
                    expertFeedbacks
                        .Where(feedback => feedback.EventId == @event.Identifier)
                        .SelectMany(feedback => feedback.Grades)
                        .GroupBy(grade => grade.CriterionId)
                        .Sum(group => group.Sum(grade => grade.GradeValue) / (float)group.Count()) +
                    participantFeedbacks
                        .Where(feedback => feedback.EventId == @event.Identifier)
                        .SelectMany(feedback => feedback.Grades)
                        .GroupBy(grade => grade.CriterionId)
                        .Sum(group => group.Sum(grade => grade.GradeValue) / (float)group.Count())))];
    }

    private bool FilterRecords(ContestEvent contestEvent)
    {
        return string.IsNullOrWhiteSpace(_searchFilter) ||
            contestEvent.Name.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase) ||
            contestEvent.Identifier.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase);
    }

    private static object SelectNameField(ContestEvent contestEvent) => contestEvent.Name;
    private static object SelectExpertFeedbacksField(ContestEvent contestEvent) => contestEvent.ExprertFeedbacksQuantity;
    private static object SelectParticipantFeedbacksField(ContestEvent contestEvent) => contestEvent.ParticipantFeedbacksQuantity;
    private static object SelectExpertGradeField(ContestEvent contestEvent) => contestEvent.ExpertGrade;
    private static object SelectParticipantGradeField(ContestEvent contestEvent) => contestEvent.ParticipantGrade;
    private static object SelectTotalGradeField(ContestEvent contestEvent) => contestEvent.TotalGrade;

    private sealed record Contest(
        string Name,
        bool RegisteredGrading,
        bool FriendlyGrading,
        bool AttendanceControl,
        int ExpertCriterionsQuantity,
        int ParticipantCriterionsQuantity,
        int EventsQuantity,
        int ExpertFeedbacksQuantity,
        int ParticipantFeedbacksQuantity);

    private sealed record ContestEvent(
        int Number,
        string Identifier,
        string Name,
        IReadOnlyCollection<string> Brigades,
        int ExprertFeedbacksQuantity,
        int ParticipantFeedbacksQuantity,
        float ExpertGrade,
        float ParticipantGrade,
        float TotalGrade);
}
