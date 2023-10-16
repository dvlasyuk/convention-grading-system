using System.Data;
using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Configuration;
using ConventionGradingSystem.Database;
using ConventionGradingSystem.Models.EventsList;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Pages;

[Authorize(Roles = "Adminstrator")]
public class EventsListModel : PageModel
{
    private readonly ApplicationConfiguration _configuration;
    private readonly DatabaseContext _databaseContext;

    public EventsListModel(
        [NotNull] IOptionsSnapshot<ApplicationConfiguration> configuration,
        [NotNull] DatabaseContext databaseContext)
    {
        _configuration = configuration.Value;
        _databaseContext = databaseContext;
    }

    public ViewModel ViewModel { get; private set; } = new ViewModel(
        ContestName: "Неизвестный конкурс",
        ExpertCriterions: new List<GradeCriterion>(),
        ParticipantCriterions: new List<GradeCriterion>(),
        Events: new List<ContestEvent>());

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

        if (contest.Events.Count == 0)
        {
            return;
        }

        var eventIds = contest.Events
            .Select(item => item.Identifier)
            .ToList();

        var expertGrades = await _databaseContext.ExpertGrades
            .Where(item => eventIds.Contains(item.EventId))
            .ToListAsync();

        var expertGradeQuantities = expertGrades
            .GroupBy(item => item.EventId)
            .ToDictionary(
                groupByEvent => groupByEvent.Key,
                groupByEvent => groupByEvent.Count());

        var expertGradeAverageValues = expertGrades
            .GroupBy(item => item.EventId)
            .ToDictionary(
                groupByEvent => groupByEvent.Key,
                groupByEvent => groupByEvent
                    .GroupBy(item => item.CriterionId)
                    .ToDictionary(
                        groupByGrade => groupByGrade.Key,
                        groupByGrade => groupByGrade.Any()
                            ? groupByGrade.Sum(item => item.GradeValue) / (float)groupByGrade.Count()
                            : 0f));

        var expertGradeTotalValues = expertGradeAverageValues
            .ToDictionary(item => item.Key, item => item.Value
                .Sum(item => item.Value));

        var participantGrades = await _databaseContext.ParticipantGrades
            .Where(item => eventIds.Contains(item.EventId))
            .ToListAsync();

        var participantGradeQuantities = participantGrades
            .GroupBy(item => item.EventId)
            .ToDictionary(
                groupByEvent => groupByEvent.Key,
                groupByEvent => groupByEvent.Count());

        var participantGradeAverageValues = participantGrades
            .GroupBy(item => item.EventId)
            .ToDictionary(
                groupByEvent => groupByEvent.Key,
                groupByEvent => groupByEvent
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
                    ExprertGradesQuantity: contest.ExpertCriterions.Count > 0
                        ? expertGradeQuantities.TryGetValue(eventItem.Identifier, out var expertGradesQuantity)
                            ? expertGradesQuantity / contest.ExpertCriterions.Count
                            : 0
                        : 0,
                    ParticipantGradesQuantity: contest.ParticipantCriterions.Count > 0
                        ? participantGradeQuantities.TryGetValue(eventItem.Identifier, out var participantGradesQuantity)
                            ? participantGradesQuantity / contest.ParticipantCriterions.Count
                            : 0
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
                        : 0f,
                    WithParticipants: eventItem.Participants.Count > 0))
                .ToList()
        };
    }
}
