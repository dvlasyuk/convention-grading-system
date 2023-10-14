using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using ConventionGradingSystem.Configuration;
using ConventionGradingSystem.Database;
using ConventionGradingSystem.Models.EventsList;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Pages
{
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

        public EventType EventType { get; set; } = new EventType
        {
            Name = "Неизвестная категория",
            ExpertGrades = new List<GradeType>(),
            ParticipantGrades = new List<GradeType>()
        };

        public List<Event> Events { get; set; } = new List<Event>();

        public async Task OnGetAsync(int eventTypeId)
        {
            var eventType = _configuration.EventTypes.FirstOrDefault(item => item.Identifier == eventTypeId);
            if (eventType == null)
            {
                return;
            }

            EventType.Identifier = eventType.Identifier;
            EventType.Name = eventType.Name;

            if (eventType.ExpertGrades?.Any() ?? false)
            {
                EventType.ExpertGrades = eventType.ExpertGrades
                    .OrderBy(item => item.Identifier)
                    .Select(item => new GradeType
                    {
                        Identifier = item.Identifier,
                        Name = item.Name
                    })
                    .ToList();
            }

            if (eventType.ParticipantGrades?.Any() ?? false)
            {
                EventType.ParticipantGrades = eventType.ParticipantGrades
                    .OrderBy(item => item.Identifier)
                    .Select(item => new GradeType
                    {
                        Identifier = item.Identifier,
                        Name = item.Name
                    })
                    .ToList();
            }

            if (!eventType.Events?.Any() ?? true)
            {
                return;
            }

            var expertGrades = await _databaseContext.ExpertGrades
                .Where(item => item.EventTypeId == eventTypeId)
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
                        .GroupBy(item => item.GradeTypeId)
                        .ToDictionary(
                            groupByGrade => groupByGrade.Key,
                            groupByGrade => groupByGrade.Any()
                                ? groupByGrade.Sum(item => item.GradeValue) / (float)groupByGrade.Count()
                                : 0f));

            var expertGradeTotalValues = expertGradeAverageValues
                .ToDictionary(item => item.Key, item => item.Value
                    .Sum(item => item.Value));

            var participantGrades = await _databaseContext.ParticipantGrades
                .Where(item => item.EventTypeId == eventTypeId)
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
                        .GroupBy(item => item.GradeTypeId)
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

            Events = eventType.Events
                .OrderBy(eventItem => eventItem.Identifier)
                .Select(eventItem => new Event
                {
                    Identifier = eventItem.Identifier,
                    Name = eventItem.Name,
                    ExprertGradesQuantity = eventType.ExpertGrades?.Any() ?? false
                        ? expertGradeQuantities.TryGetValue(eventItem.Identifier, out var expertGradesQuantity)
                            ? expertGradesQuantity / eventType.ExpertGrades.Count
                            : 0
                        : 0,
                    ParticipantGradesQuantity = eventType.ParticipantGrades?.Any() ?? false
                        ? participantGradeQuantities.TryGetValue(eventItem.Identifier, out var participantGradesQuantity)
                            ? participantGradesQuantity / eventType.ParticipantGrades.Count
                            : 0
                        : 0,
                    ExpertGrades = eventType.ExpertGrades?.Any() ?? false
                        ? eventType.ExpertGrades
                            .OrderBy(gradeItem => gradeItem.Identifier)
                            .ToDictionary(
                                gradeItem => gradeItem.Identifier,
                                gradeItem => expertGradeAverageValues.TryGetValue(eventItem.Identifier, out var expertGrades)
                                    ? expertGrades.TryGetValue(gradeItem.Identifier, out var expertGrade)
                                        ? expertGrade
                                        : 0f
                                    : 0f)
                        : new Dictionary<int, float>(),
                    ParticipantGrades = eventType.ParticipantGrades?.Any() ?? false
                        ? eventType.ParticipantGrades
                            .OrderBy(gradeItem => gradeItem.Identifier)
                            .ToDictionary(
                                gradeItem => gradeItem.Identifier,
                                gradeItem => participantGradeAverageValues.TryGetValue(eventItem.Identifier, out var participantGrades)
                                    ? participantGrades.TryGetValue(gradeItem.Identifier, out var participantGrade)
                                        ? participantGrade
                                        : 0f
                                    : 0f)
                        : new Dictionary<int, float>(),
                    TotalExpertGrade = expertGradeTotalValues.TryGetValue(eventItem.Identifier, out var totalExpertGrade)
                        ? totalExpertGrade
                        : 0f,
                    TotalParticipantGrade = participantGradeTotalValues.TryGetValue(eventItem.Identifier, out var totalParticipantGrade)
                        ? totalParticipantGrade
                        : 0f,
                    TotalGrade = totalGradeValues.TryGetValue(eventItem.Identifier, out var totalGrade)
                        ? totalGrade
                        : 0f,
                    WithParticipants = eventItem.Participants?.Any() ?? false
                })
                .ToList();
        }
    }
}
