using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.Host.Components.Administration.Contests.Models;
using ConventionGradingSystem.Host.Components.Shared.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Administration.Contests.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class EventsList
{
    private readonly DatabaseContext _databaseContext;
    private readonly IOptions<DataConfiguration> _dataConfiguration;

    [Parameter]
    public string? ContestId { get; set; }

    public EventsList(
        DatabaseContext databaseContext,
        IOptions<DataConfiguration> dataConfiguration)
    {
        _databaseContext = databaseContext;
        _dataConfiguration = dataConfiguration;
    }

    protected override async Task<ContentState<EventsListModel>> LoadContent()
    {
        var contest = _dataConfiguration.Value.Contests.FirstOrDefault(item => item.Identifier == ContestId);
        if (contest == null)
        {
            return Shared.Models.LoadingError.ContestNotFound;
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

        var viewContest = new EventsListModel.ContestModel(
            Name: contest.Name,
            RegisteredGrading: contest.RegisteredGrading,
            FriendlyGrading: contest.FriendlyGrading,
            AttendanceControl: contest.AttendanceControl,
            ExpertCriterionsQuantity: contest.ExpertCriterions.Count,
            ParticipantCriterionsQuantity: contest.ParticipantCriterions.Count,
            EventsQuantity: contest.Events.Count,
            ExpertFeedbacksQuantity: expertFeedbacks.Count,
            ParticipantFeedbacksQuantity: participantFeedbacks.Count);

        var brigades = _dataConfiguration.Value.Brigades.ToDictionary(brigade => brigade.Identifier);
        var viewEvents = contest.Events
            .OrderBy(@event => @event.Identifier)
            .Select((@event, index) => new EventsListModel.EventModel(
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
                        .Sum(group => group.Sum(grade => grade.GradeValue) / (float)group.Count()),
                ExpertFeedbackFormLink: $"/grading/events/{@event.Identifier}/expert-feedback",
                ExpertFeedbacksListLink: $"/administration/events/{@event.Identifier}/expert-feedbacks",
                ParticipantFeedbackFormLink: $"/grading/events/{@event.Identifier}/participant-feedback",
                ParticipantFeedbacksListLink: $"/administration/events/{@event.Identifier}/participant-feedbacks",
                FacilitatorAttendanceFormLink: $"/grading/events/{@event.Identifier}/facilitator-attendance"))
            .ToArray();

        return new EventsListModel(
            Contest: viewContest,
            Events: viewEvents,
            SearchFilter: null);
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
