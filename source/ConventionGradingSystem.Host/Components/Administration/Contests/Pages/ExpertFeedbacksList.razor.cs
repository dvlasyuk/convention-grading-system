using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Administration.Contests.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class ExpertFeedbacksList
{
    private readonly DatabaseContext _databaseContext;
    private readonly IOptions<ApplicationConfiguration> _configuration;

    private ContestEvent? _contestEvent;
    private IEnumerable<Feedback> _feedbacks = [];
    private string? _searchFilter;

    [Parameter]
    public string? EventId { get; set; }

    public ExpertFeedbacksList(
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

        var feedbacks = await _databaseContext.ExpertFeedbacks
            .Where(item => item.EventId == EventId)
            .Include(item => item.Grades)
            .ToListAsync();

        var contest = _configuration.Value.Contests.First(item => item.Events.Contains(contestEvent));
        _contestEvent = new ContestEvent(
            EventName: contestEvent.Name,
            ContestName: contest.Name,
            IsGraded: contest.ExpertCriterions.Count > 0,
            FeedbacksQuantity: feedbacks.Count,
            GradeCriterions: [.. contest.ExpertCriterions
                .Select(criterion => new GradeCriterion(
                    Identifier: criterion.Identifier,
                    Name: criterion.Name,
                    TotalGrade: feedbacks.Count == 0 ? 0f :
                        feedbacks
                            .SelectMany(feedback => feedback.Grades)
                            .Where(grade => grade.CriterionId == criterion.Identifier)
                            .Sum(grade => grade.GradeValue) /
                        (float)feedbacks.Count))]);

        var experts = _configuration.Value.Experts.ToDictionary(item => item.Identifier);
        _feedbacks = [.. feedbacks
            .OrderBy(feedback => feedback.Identifier)
            .Select((feedback, index) => new Feedback(
                Number: index + 1,
                ExpertName: experts[feedback.ExpertId].Name,
                Grades: feedback.Grades.ToDictionary(
                    grade => grade.CriterionId,
                    grade => grade.GradeValue),
                Note: feedback.Note,
                ReceivedAt: feedback.ReceivedAt.ToString(CultureInfo.CurrentCulture)))];
    }

    private bool FilterRecords(Feedback feedback)
    {
        return string.IsNullOrWhiteSpace(_searchFilter) ||
            feedback.ExpertName.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase) ||
            (feedback.Note?.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private static object SelectNameField(Feedback feedback) => feedback.ExpertName;
    private static object SelectGradeField(Feedback feedback, string gradeId) => feedback.Grades[gradeId];

    private sealed record ContestEvent(
        string EventName,
        string ContestName,
        bool IsGraded,
        int FeedbacksQuantity,
        IReadOnlyList<GradeCriterion> GradeCriterions);

    private sealed record GradeCriterion(
        string Identifier,
        string Name,
        float TotalGrade);

    private sealed record Feedback(
        int Number,
        string ExpertName,
        IReadOnlyDictionary<string, int> Grades,
        string? Note,
        string ReceivedAt);
}
