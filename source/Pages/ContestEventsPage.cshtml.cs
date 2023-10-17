using System.Data;
using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Configuration;
using ConventionGradingSystem.Database;
using ConventionGradingSystem.Models.ContestEventsPage;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Pages;

/// <summary>
/// Модель страницы приложения со списком мероприятий в рамках конкурса мероприятий.
/// </summary>
[Authorize(Roles = "Adminstrator")]
public class ContestEventsPageModel : PageModel
{
    private readonly ApplicationConfiguration _configuration;
    private readonly DatabaseContext _databaseContext;

    /// <summary>
    /// Создаёт новый экземпляр <see cref="ContestEventsPageModel"/>.
    /// </summary>
    /// <param name="configuration">Конфигурационные данные приложения.</param>
    /// <param name="databaseContext">Контекст для доступа к базе данных.</param>
    public ContestEventsPageModel(
        [NotNull] IOptionsSnapshot<ApplicationConfiguration> configuration,
        [NotNull] DatabaseContext databaseContext)
    {
        _configuration = configuration.Value;
        _databaseContext = databaseContext;
    }

    /// <summary>
    /// Модель представления страницы.
    /// </summary>
    public ViewModel ViewModel { get; private set; } = new ViewModel(
        ContestName: "Неизвестный конкурс",
        ExpertCriterions: new List<GradeCriterion>(),
        ParticipantCriterions: new List<GradeCriterion>(),
        Events: new List<ContestEvent>());

    /// <summary>
    /// Обрабатывает GET-запрос к странице.
    /// </summary>
    /// <param name="contestId">Идентификатор конкурса мероприятий.</param>
    public async Task OnGetAsync(string contestId)
    {
        var contest = _configuration.Contests.FirstOrDefault(item => item.Identifier == contestId);
        if (contest == null)
        {
            return;
        }

        ViewModel = ViewModel with
        {
            ContestName = contest.Name,
            ExpertCriterions = contest.ExpertCriterions
                .OrderBy(item => item.Identifier)
                .Select(item => new GradeCriterion(
                    Identifier: item.Identifier,
                    Name: item.Name))
                .ToList(),
            ParticipantCriterions = contest.ParticipantCriterions
                .OrderBy(item => item.Identifier)
                .Select(item => new GradeCriterion(
                    Identifier: item.Identifier,
                    Name: item.Name))
                .ToList()
        };

        var eventIds = contest.Events
            .Select(item => item.Identifier)
            .ToList();

        var expertFeedbacks = await _databaseContext.ExpertFeedbacks
            .Where(item => eventIds.Contains(item.EventId))
            .Include(item => item.Grades)
            .ToListAsync();

        var expertFeedbackQuantities = expertFeedbacks
            .GroupBy(item => item.EventId)
            .ToDictionary(
                groupByEvent => groupByEvent.Key,
                groupByEvent => groupByEvent.Count());

        var expertGradeAverageValues = expertFeedbacks
            .GroupBy(item => item.EventId)
            .ToDictionary(
                groupByEvent => groupByEvent.Key,
                groupByEvent => groupByEvent
                    .SelectMany(item => item.Grades)
                    .GroupBy(item => item.CriterionId)
                    .ToDictionary(
                        groupByGrade => groupByGrade.Key,
                        groupByGrade => groupByGrade.Any()
                            ? groupByGrade.Sum(item => item.GradeValue) / (float)groupByGrade.Count()
                            : 0f));

        var expertGradeTotalValues = expertGradeAverageValues
            .ToDictionary(item => item.Key, item => item.Value
                .Sum(item => item.Value));

        var participantFeedbacks = await _databaseContext.ParticipantFeedbacks
            .Where(item => eventIds.Contains(item.EventId))
            .Include(item => item.Grades)
            .ToListAsync();

        var participantFeedbacksQuantities = participantFeedbacks
            .GroupBy(item => item.EventId)
            .ToDictionary(
                groupByEvent => groupByEvent.Key,
                groupByEvent => groupByEvent.Count());

        var participantGradeAverageValues = participantFeedbacks
            .GroupBy(item => item.EventId)
            .ToDictionary(
                groupByEvent => groupByEvent.Key,
                groupByEvent => groupByEvent
                    .SelectMany(item => item.Grades)
                    .GroupBy(item => item.CriterionId)
                    .ToDictionary(
                        groupByGrade => groupByGrade.Key,
                        groupByGrade => groupByGrade.Any()
                            ? groupByGrade.Sum(item => item.GradeValue) / (float)groupByGrade.Count()
                            : 0f));

        var participantGradeTotalValues = participantGradeAverageValues
            .ToDictionary(item => item.Key, item => item.Value
                .Sum(item => item.Value));

        var totalGradeValues = expertGradeTotalValues
            .Union(participantGradeTotalValues)
            .GroupBy(item => item.Key)
            .ToDictionary(
                group => group.Key,
                group => group.Sum(item => item.Value));

        ViewModel = ViewModel with
        {
            Events = contest.Events
                .OrderBy(eventItem => eventItem.Identifier)
                .Select(eventItem => new ContestEvent(
                    Identifier: eventItem.Identifier,
                    Name: eventItem.Name,
                    ExprertFeedbacksQuantity: expertFeedbackQuantities.TryGetValue(eventItem.Identifier, out var expertFeedbacksQuantity)
                        ? expertFeedbacksQuantity
                        : 0,
                    ParticipantFeedbacksQuantity: participantFeedbacksQuantities.TryGetValue(eventItem.Identifier, out var participantGradesQuantity)
                        ? participantGradesQuantity
                        : 0,
                    ExpertGrades: contest.ExpertCriterions.Count > 0
                        ? contest.ExpertCriterions
                            .OrderBy(gradeItem => gradeItem.Identifier)
                            .ToDictionary(
                                gradeItem => gradeItem.Identifier,
                                gradeItem => expertGradeAverageValues.TryGetValue(eventItem.Identifier, out var expertGrades)
                                    ? expertGrades.TryGetValue(gradeItem.Identifier, out var expertGrade)
                                        ? expertGrade
                                        : 0f
                                    : 0f)
                        : new Dictionary<string, float>(),
                    ParticipantGrades: contest.ParticipantCriterions.Count > 0
                        ? contest.ParticipantCriterions
                            .OrderBy(gradeItem => gradeItem.Identifier)
                            .ToDictionary(
                                gradeItem => gradeItem.Identifier,
                                gradeItem => participantGradeAverageValues.TryGetValue(eventItem.Identifier, out var participantGrades)
                                    ? participantGrades.TryGetValue(gradeItem.Identifier, out var participantGrade)
                                        ? participantGrade
                                        : 0f
                                    : 0f)
                        : new Dictionary<string, float>(),
                    TotalExpertGrade: expertGradeTotalValues.TryGetValue(eventItem.Identifier, out var totalExpertGrade)
                        ? totalExpertGrade
                        : 0f,
                    TotalParticipantGrade: participantGradeTotalValues.TryGetValue(eventItem.Identifier, out var totalParticipantGrade)
                        ? totalParticipantGrade
                        : 0f,
                    TotalGrade: totalGradeValues.TryGetValue(eventItem.Identifier, out var totalGrade)
                        ? totalGrade
                        : 0f))
                .ToList()
        };
    }
}
