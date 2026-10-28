using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.Host.Components.Administration.Contests.Models;
using ConventionGradingSystem.Host.Components.Shared.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Administration.Contests.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class ParticipantFeedbacksList
{
    private readonly IDbContextFactory<DatabaseContext> _databaseContextFactory;
    private readonly IOptions<DataConfiguration> _dataConfiguration;

    [Parameter]
    public string? EventId { get; set; }

    public ParticipantFeedbacksList(
        IDbContextFactory<DatabaseContext> databaseContextFactory,
        IOptions<DataConfiguration> dataConfiguration)
    {
        _databaseContextFactory = databaseContextFactory;
        _dataConfiguration = dataConfiguration;
    }

    protected override async Task<ContentState<ParticipantFeedbacksListModel>> LoadContent()
    {
        var contestEvent = _dataConfiguration.Value.Contests
            .SelectMany(item => item.Events)
            .FirstOrDefault(item => item.Identifier == EventId);

        if (contestEvent == null)
        {
            return Shared.Models.LoadingError.EventNotFound;
        }

        using var databaseContext = _databaseContextFactory.CreateDbContext();
        var feedbacks = await databaseContext.ParticipantFeedbacks
            .Where(item => item.EventId == EventId)
            .Include(item => item.Grades)
            .ToListAsync();

        var contest = _dataConfiguration.Value.Contests.First(item => item.Events.Contains(contestEvent));
        var brigades = _dataConfiguration.Value.Brigades.ToDictionary(brigade => brigade.Identifier);
        var teams = _dataConfiguration.Value.Teams.ToDictionary(team => team.Identifier);

        var viewEvent = new ParticipantFeedbacksListModel.EventModel(
            EventName: contestEvent.Name,
            ContestName: contest.Name,
            IsGraded: contest.ParticipantCriterions.Count > 0,
            Brigades: [.. contestEvent.Brigades
                .Select(identifier => brigades[identifier])
                .Select(brigade => brigade.Name)],
            FeedbacksQuantity: feedbacks.Count,
            GradeCriterions: [.. contest.ParticipantCriterions
                .Select(criterion => new ParticipantFeedbacksListModel.CriterionModel(
                    Identifier: criterion.Identifier,
                    Name: criterion.Name,
                    TotalGrade: feedbacks.Count == 0 ? 0f :
                        feedbacks
                            .SelectMany(feedback => feedback.Grades)
                            .Where(grade => grade.CriterionId == criterion.Identifier)
                            .Sum(grade => grade.GradeValue) /
                        (float)feedbacks.Count))]);

        var participants = _dataConfiguration.Value.Participants.ToDictionary(item => item.Identifier);
        var viewFeedbacks = feedbacks
            .OrderBy(feedback => feedback.Identifier)
            .Select((feedback, index) => new ParticipantFeedbacksListModel.FeedbackModel(
                Number: index + 1,
                ParticipantName: participants[feedback.ParticipantId].Name,
                ParticipantBrigade: !string.IsNullOrWhiteSpace(participants[feedback.ParticipantId].Brigade)
                    ? brigades[participants[feedback.ParticipantId].Brigade!].Name
                    : null,
                ParticipantTeam: teams[participants[feedback.ParticipantId].Team].Name,
                Grades: feedback.Grades.ToDictionary(
                    grade => grade.CriterionId,
                    grade => grade.GradeValue),
                Note: feedback.Note,
                ReceivedAt: feedback.ReceivedAt.ToString(CultureInfo.CurrentCulture)))
            .ToArray();

        return new ParticipantFeedbacksListModel(
            Event: viewEvent,
            Feedbacks: viewFeedbacks,
            SearchFilter: null);
    }

    private bool FilterRecords(ParticipantFeedbacksListModel.FeedbackModel feedback)
    {
        return string.IsNullOrWhiteSpace(ViewModel.SearchFilter) ||
            feedback.ParticipantName.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            feedback.ParticipantTeam.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            (feedback.ParticipantBrigade?.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (feedback.Note?.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private static object SelectNameField(ParticipantFeedbacksListModel.FeedbackModel feedback) => feedback.ParticipantName;
    private static object SelectBrigadeField(ParticipantFeedbacksListModel.FeedbackModel feedback) => feedback.ParticipantBrigade ?? string.Empty;
    private static object SelectTeamField(ParticipantFeedbacksListModel.FeedbackModel feedback) => feedback.ParticipantTeam;

    private static object SelectGradeField(ParticipantFeedbacksListModel.FeedbackModel feedback, string gradeId) => feedback.Grades[gradeId];
}
