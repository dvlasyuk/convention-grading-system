using ClosedXML.Excel;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Services.Export.Contests;

public class ExpertFeedbacksListExporter : BaseExporter
{
    private readonly IDbContextFactory<DatabaseContext> _databaseContextFactory;
    private readonly IOptions<DataConfiguration> _dataConfiguration;

    public ExpertFeedbacksListExporter(
        IDbContextFactory<DatabaseContext> databaseContextFactory,
        IOptions<DataConfiguration> dataConfiguration)
    {
        _databaseContextFactory = databaseContextFactory;
        _dataConfiguration = dataConfiguration;
    }

    public async Task<byte[]> ExportToExcel(string eventId)
    {
        var (criterions, feedbacks) = await LoadData(eventId);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("Отзывы экспертов");

        var rowNumber = 1;
        var columnNumber = 1;

        FillCell(worksheet, rowNumber, ref columnNumber, "#");
        FillCell(worksheet, rowNumber, ref columnNumber, "Имя эксперта");

        foreach (var criterion in criterions)
        {
            FillCell(worksheet, rowNumber, ref columnNumber, criterion);
        }

        FillCell(worksheet, rowNumber, ref columnNumber, "Итоговая оценка");
        FillCell(worksheet, rowNumber, ref columnNumber, "Дополнительный комментарий");
        FillCell(worksheet, rowNumber, ref columnNumber, "Время получения");

        rowNumber++;

        foreach (var feedback in feedbacks)
        {
            columnNumber = 1;

            FillCell(worksheet, rowNumber, ref columnNumber, feedback.Number);
            FillCell(worksheet, rowNumber, ref columnNumber, feedback.ExpertName);

            foreach (var grade in feedback.Grades)
            {
                FillCell(worksheet, rowNumber, ref columnNumber, grade);
            }

            FillCell(worksheet, rowNumber, ref columnNumber, feedback.TotalGrade);
            FillCell(worksheet, rowNumber, ref columnNumber, feedback.Note);
            FillCell(worksheet, rowNumber, ref columnNumber, feedback.ReceivedAt);

            rowNumber++;
        }

        FormatTable(
            worksheet: worksheet,
            startRowNumber: 1,
            startColumnNumber: 1,
            tableRowsCount: rowNumber - 1,
            tableColumnsCount: columnNumber - 1,
            headerRowsCount: 1);

        return ExportWorkbook(workbook);
    }

    private async Task<(string[], FeedbackData[])> LoadData(string eventId)
    {
        var contestEvent = _dataConfiguration.Value.Contests
                .SelectMany(item => item.Events)
                .FirstOrDefault(item => item.Identifier == eventId)
            ?? throw new InvalidOperationException($"Event {eventId} is not found");

        var contest = _dataConfiguration.Value.Contests.First(item => item.Events.Contains(contestEvent));
        if (contest.ExpertCriterions.Count == 0)
        {
            throw new InvalidOperationException($"Event {eventId} is not graded by experts");
        }

        var criterionsData = contest.ExpertCriterions
            .OrderBy(criterion => criterion.Identifier)
            .Select(criterion => criterion.Name)
            .ToArray();

        using var databaseContext = _databaseContextFactory.CreateDbContext();
        var feedbacks = await databaseContext.ExpertFeedbacks
            .Where(item => item.EventId == contestEvent.Identifier)
            .Include(item => item.Grades)
            .ToListAsync();

        var experts = _dataConfiguration.Value.Experts.ToDictionary(item => item.Identifier);
        var feedbacksData = feedbacks
            .OrderBy(feedback => feedback.Identifier)
            .Select((feedback, index) => new FeedbackData(
                Number: index + 1,
                ExpertName: experts[feedback.ExpertId].Name,
                Grades: [.. feedback.Grades
                    .OrderBy(grade => grade.CriterionId)
                    .Select(grade => grade.GradeValue)],
                TotalGrade: feedback.Grades
                    .Sum(grade => grade.GradeValue),
                Note: feedback.Note,
                ReceivedAt: feedback.ReceivedAt.DateTime))
            .ToArray();

        return (criterionsData, feedbacksData);
    }

    private sealed record FeedbackData(
        int Number,
        string ExpertName,
        int[] Grades,
        int TotalGrade,
        string? Note,
        DateTime ReceivedAt);
}
