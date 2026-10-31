using ClosedXML.Excel;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Services.Export.Participants;

public class TeamsListExporter : BaseExporter
{
    private readonly IDbContextFactory<DatabaseContext> _databaseContextFactory;
    private readonly IOptions<DataConfiguration> _dataConfiguration;

    public TeamsListExporter(
        IDbContextFactory<DatabaseContext> databaseContextFactory,
        IOptions<DataConfiguration> dataConfiguration)
    {
        _databaseContextFactory = databaseContextFactory;
        _dataConfiguration = dataConfiguration;
    }

    public async Task<byte[]> ExportToExcel()
    {
        var teams = await LoadData();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("Отзывы участников");

        var rowNumber = 1;
        var columnNumber = 1;

        FillCell(worksheet, rowNumber, ref columnNumber, "#");
        FillCell(worksheet, rowNumber, ref columnNumber, "Название команды");
        FillCell(worksheet, rowNumber, ref columnNumber, "Количество участников");
        FillCell(worksheet, rowNumber, ref columnNumber, "Количество регистраций");
        FillCell(worksheet, rowNumber, ref columnNumber, "Количество посещений");
        FillCell(worksheet, rowNumber, ref columnNumber, "Количество пропусков");
        FillCell(worksheet, rowNumber, ref columnNumber, "Особые отметки");

        rowNumber++;

        foreach (var team in teams)
        {
            columnNumber = 1;

            FillCell(worksheet, rowNumber, ref columnNumber, team.Number);
            FillCell(worksheet, rowNumber, ref columnNumber, team.TeamName);
            FillCell(worksheet, rowNumber, ref columnNumber, team.MembersQuantity);
            FillCell(worksheet, rowNumber, ref columnNumber, team.RegistrationsQuantity);
            FillCell(worksheet, rowNumber, ref columnNumber, team.AttendancesQuantity);
            FillCell(worksheet, rowNumber, ref columnNumber, team.AbsencesQuantity);
            FillCell(worksheet, rowNumber, ref columnNumber, team.SpecialMarksQuantity);

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

    private async Task<TeamData[]> LoadData()
    {
        using var databaseContext = _databaseContextFactory.CreateDbContext();
        var attendanceMarks = await databaseContext.AttendanceMarks.ToArrayAsync();

        var teamsData = _dataConfiguration.Value.Teams
            .OrderBy(team => team.Identifier)
            .Select((team, index) => new TeamData(
                Number: index + 1,
                TeamName: team.Name,
                MembersQuantity: _dataConfiguration.Value.Participants
                    .Count(participant => participant.Team == team.Identifier),
                RegistrationsQuantity: _dataConfiguration.Value.Participants
                    .Where(participant => participant.Team == team.Identifier)
                    .SelectMany(participant => _dataConfiguration.Value.Contests
                        .SelectMany(contest => contest.Events)
                        .SelectMany(@event => @event.Participants)
                        .Where(identifier => identifier == participant.Identifier))
                    .Count(),
                AttendancesQuantity: _dataConfiguration.Value.Participants
                    .Where(participant => participant.Team == team.Identifier)
                    .SelectMany(participant => attendanceMarks
                        .Where(mark => mark.ParticipantId == participant.Identifier))
                    .Count(),
                AbsencesQuantity:
                    _dataConfiguration.Value.Participants
                        .Where(participant => participant.Team == team.Identifier)
                        .SelectMany(participant => _dataConfiguration.Value.Contests
                            .SelectMany(contest => contest.Events)
                            .SelectMany(@event => @event.Participants)
                            .Where(identifier => identifier == participant.Identifier))
                        .Count() -
                    _dataConfiguration.Value.Participants
                        .Where(participant => participant.Team == team.Identifier)
                        .SelectMany(participant => attendanceMarks
                            .Where(mark => mark.ParticipantId == participant.Identifier))
                        .Count(),
                SpecialMarksQuantity: _dataConfiguration.Value.Participants
                    .Where(participant => participant.Team == team.Identifier)
                    .SelectMany(participant => attendanceMarks
                        .Where(mark => mark.ParticipantId == participant.Identifier))
                    .Count(mark => mark.SpecialMark)))
            .ToArray();

        return teamsData;
    }

    private sealed record TeamData(
        int Number,
        string TeamName,
        int MembersQuantity,
        int RegistrationsQuantity,
        int AttendancesQuantity,
        int AbsencesQuantity,
        int SpecialMarksQuantity);
}
