using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.Host.Components.Administration.Contests.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Administration.Contests.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class ParticipantFeedbacksList
{
    private readonly DatabaseContext _databaseContext;
    private readonly IOptions<ApplicationConfiguration> _configuration;

    private readonly ParticipantFeedbacksListModel _viewModel = new();

    [Parameter]
    public string? EventId { get; set; }

    public ParticipantFeedbacksList(
        DatabaseContext databaseContext,
        IOptions<ApplicationConfiguration> configuration)
    {
        _databaseContext = databaseContext;
        _configuration = configuration;
    }

    protected override async Task OnInitializedAsync()
    {
        var contestEvent = _configuration.Value.Contests
            .SelectMany(item => item.Events)
            .FirstOrDefault(item => item.Identifier == EventId);

        if (contestEvent == null)
        {
            return;
        }

        var feedbacks = await _databaseContext.ParticipantFeedbacks
            .Where(item => item.EventId == EventId)
            .Include(item => item.Grades)
            .ToListAsync();

        var contest = _configuration.Value.Contests.First(item => item.Events.Contains(contestEvent));
        var brigades = _configuration.Value.Brigades.ToDictionary(brigade => brigade.Identifier);
        var teams = _configuration.Value.Teams.ToDictionary(team => team.Identifier);

        _viewModel.Event = new ParticipantFeedbacksListModel.EventModel(
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

        var participants = _configuration.Value.Participants.ToDictionary(item => item.Identifier);
        _viewModel.Feedbacks = [.. feedbacks
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
                ReceivedAt: feedback.ReceivedAt.ToString(new CultureInfo("ru-RU"))))];
    }

    private bool FilterRecords(ParticipantFeedbacksListModel.FeedbackModel feedback)
    {
        return string.IsNullOrWhiteSpace(_viewModel.SearchFilter) ||
            feedback.ParticipantName.Contains(_viewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            feedback.ParticipantTeam.Contains(_viewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            (feedback.ParticipantBrigade?.Contains(_viewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (feedback.Note?.Contains(_viewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private static object SelectNameField(ParticipantFeedbacksListModel.FeedbackModel feedback) => feedback.ParticipantName;
    private static object SelectBrigadeField(ParticipantFeedbacksListModel.FeedbackModel feedback) => feedback.ParticipantBrigade ?? string.Empty;
    private static object SelectTeamField(ParticipantFeedbacksListModel.FeedbackModel feedback) => feedback.ParticipantTeam;

    private static object SelectGradeField(ParticipantFeedbacksListModel.FeedbackModel feedback, string gradeId) => feedback.Grades[gradeId];
}
