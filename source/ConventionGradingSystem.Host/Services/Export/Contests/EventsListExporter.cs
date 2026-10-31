using ClosedXML.Excel;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Services.Export.Contests;

public class EventsListExporter : BaseExporter
{
    private readonly IDbContextFactory<DatabaseContext> _databaseContextFactory;
    private readonly IOptions<DataConfiguration> _dataConfiguration;

    public EventsListExporter(
        IDbContextFactory<DatabaseContext> databaseContextFactory,
        IOptions<DataConfiguration> dataConfiguration)
    {
        _databaseContextFactory = databaseContextFactory;
        _dataConfiguration = dataConfiguration;
    }

    public async Task<byte[]> ExportToExcel(string contestId)
    {
        var (contest, events) = await LoadData(contestId);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("Мероприятия конкурса");

        var rowNumber = 1;
        var columnNumber = 1;

        FillCellsGroup(worksheet, rowNumber, ref columnNumber, 2, "#");
        FillCellsGroup(worksheet, rowNumber, ref columnNumber, 2, "Название мероприятия");
        FillCellsGroup(worksheet, rowNumber, ref columnNumber, 2, "Связанные отряды");

        if (contest.ExpertCriterions.Length > 0)
        {
            FillCellsGroup(worksheet, rowNumber, ref columnNumber, 2, "Отзывы экспертов");
        }
        if (contest.ParticipantCriterions.Length > 0)
        {
            FillCellsGroup(worksheet, rowNumber, ref columnNumber, 2, "Отзывы участников");
        }

        if (contest.ExpertCriterions.Length > 0)
        {
            FillCellsGroup(
                worksheet: worksheet,
                rowNumber: rowNumber,
                columnNumber: ref columnNumber,
                groupValue: "Оценки экспертов",
                values: [.. contest.ExpertCriterions, "Итоговая оценка экспертов"]);
        }
        if (contest.ParticipantCriterions.Length > 0)
        {
            FillCellsGroup(
                worksheet: worksheet,
                rowNumber: rowNumber,
                columnNumber: ref columnNumber,
                groupValue: "Оценки участников",
                values: [.. contest.ParticipantCriterions, "Итоговая оценка участников"]);
        }

        FillCellsGroup(worksheet, 1, ref columnNumber, 2, "Общая итоговая оценка");

        rowNumber += 2;

        foreach (var eventData in events)
        {
            columnNumber = 1;

            FillCell(worksheet, rowNumber, ref columnNumber, eventData.EventNumber);
            FillCell(worksheet, rowNumber, ref columnNumber, eventData.EventName);
            FillCell(worksheet, rowNumber, ref columnNumber, string.Join(Environment.NewLine, eventData.Brigades));

            if (contest.ExpertCriterions.Length > 0)
            {
                FillCell(worksheet, rowNumber, ref columnNumber, eventData.ExpertFeedbacksQuantity);
            }
            if (contest.ParticipantCriterions.Length > 0)
            {
                FillCell(worksheet, rowNumber, ref columnNumber, eventData.ParticipantFeedbacksQuantity);
            }

            if (contest.ExpertCriterions.Length > 0)
            {
                foreach (var grade in eventData.ExpertGrades)
                {
                    FillCell(worksheet, rowNumber, ref columnNumber, grade);
                }

                FillCell(worksheet, rowNumber, ref columnNumber, eventData.ExpertTotalGrade);
            }
            if (contest.ParticipantCriterions.Length > 0)
            {
                foreach (var grade in eventData.ParticipantGrades)
                {
                    FillCell(worksheet, rowNumber, ref columnNumber, grade);
                }

                FillCell(worksheet, rowNumber, ref columnNumber, eventData.ParticipantTotalGrade);
            }

            FillCell(worksheet, rowNumber, ref columnNumber, eventData.TotalGrade);

            rowNumber++;
        }

        FormatTable(
            worksheet: worksheet,
            startRowNumber: 1,
            startColumnNumber: 1,
            tableRowsCount: rowNumber - 1,
            tableColumnsCount: columnNumber - 1,
            headerRowsCount: 2);

        return ExportWorkbook(workbook);
    }

    private async Task<(ContestData, EventData[])> LoadData(string contestId)
    {
        var contest = _dataConfiguration.Value.Contests.FirstOrDefault(item => item.Identifier == contestId)
            ?? throw new InvalidOperationException($"Contest {contestId} is not found");

        var contestData = new ContestData(
            ExpertCriterions: [.. contest.ExpertCriterions
                .OrderBy(criterion => criterion.Identifier)
                .Select(criterion => criterion.Name)],
            ParticipantCriterions: [.. contest.ParticipantCriterions
                .OrderBy(criterion => criterion.Identifier)
                .Select(criterion => criterion.Name)]);

        var contestEvents = contest.Events
            .OrderBy(item => item.Identifier)
            .ToArray();

        var eventIds = contest.Events
            .Select(item => item.Identifier)
            .ToHashSet();

        var brigades = _dataConfiguration.Value.Brigades.ToDictionary(brigade => brigade.Identifier);

        using var databaseContext = _databaseContextFactory.CreateDbContext();

        var allExpertFeedbacks = await databaseContext.ExpertFeedbacks
            .Where(item => eventIds.Contains(item.EventId))
            .Include(item => item.Grades)
            .ToListAsync();

        var allParticipantFeedbacks = await databaseContext.ParticipantFeedbacks
            .Where(item => eventIds.Contains(item.EventId))
            .Include(item => item.Grades)
            .ToListAsync();

        var expertFeedbacksByEventId = allExpertFeedbacks
            .GroupBy(item => item.EventId)
            .ToDictionary(group => group.Key, group => group.ToList());

        var participantFeedbacksByEventId = allParticipantFeedbacks
            .GroupBy(item => item.EventId)
            .ToDictionary(group => group.Key, group => group.ToList());

        var eventsData = new List<EventData>();
        for (var eventNumber = 0; eventNumber < contestEvents.Length; eventNumber++)
        {
            var contestEvent = contestEvents[eventNumber];
            var eventBrigades = contestEvent.Brigades
                .Select(identifier => brigades[identifier])
                .Select(brigade => brigade.Name)
                .ToArray();

            var expertFeedbacks = expertFeedbacksByEventId.GetValueOrDefault(contestEvent.Identifier, []);
            var participantFeedbacks = participantFeedbacksByEventId.GetValueOrDefault(contestEvent.Identifier, []);

            var expertGrades = new List<float>();
            foreach (var criterion in contest.ExpertCriterions.OrderBy(item => item.Identifier))
            {
                var criterionGrades = expertFeedbacks
                    .SelectMany(feedback => feedback.Grades)
                    .Where(grade => grade.CriterionId == criterion.Identifier)
                    .Select(grade => grade.GradeValue)
                    .ToList();

                expertGrades.Add(criterionGrades.Count > 0
                    ? criterionGrades.Sum() / (float)criterionGrades.Count
                    : 0f);
            }

            var participantGrades = new List<float>();
            foreach (var criterion in contest.ParticipantCriterions.OrderBy(item => item.Identifier))
            {
                var criterionGrades = participantFeedbacks
                    .SelectMany(feedback => feedback.Grades)
                    .Where(grade => grade.CriterionId == criterion.Identifier)
                    .Select(grade => grade.GradeValue)
                    .ToList();

                participantGrades.Add(criterionGrades.Count > 0
                    ? criterionGrades.Sum() / (float)criterionGrades.Count
                    : 0f);
            }

            eventsData.Add(new EventData(
                EventNumber: eventNumber + 1,
                EventName: contestEvent.Name,
                Brigades: eventBrigades,
                ExpertFeedbacksQuantity: expertFeedbacks.Count,
                ParticipantFeedbacksQuantity: participantFeedbacks.Count,
                ExpertGrades: [.. expertGrades],
                ParticipantGrades: [.. participantGrades],
                ExpertTotalGrade: expertGrades.Sum(),
                ParticipantTotalGrade: participantGrades.Sum(),
                TotalGrade: expertGrades.Sum() + participantGrades.Sum()));
        }

        return (contestData, eventsData.ToArray());
    }

    private sealed record ContestData(
        string[] ExpertCriterions,
        string[] ParticipantCriterions);

    private sealed record EventData(
        int EventNumber,
        string EventName,
        string[] Brigades,
        int ExpertFeedbacksQuantity,
        int ParticipantFeedbacksQuantity,
        float[] ExpertGrades,
        float[] ParticipantGrades,
        float ExpertTotalGrade,
        float ParticipantTotalGrade,
        float TotalGrade);
}
