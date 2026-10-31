using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.Host.Components.Administration.Participants.Models;
using ConventionGradingSystem.Host.Components.Shared.Models;
using ConventionGradingSystem.Host.Services;
using ConventionGradingSystem.Host.Services.Export.Participants;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Administration.Participants.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class TeamsList
{
    private readonly IDbContextFactory<DatabaseContext> _databaseContextFactory;
    private readonly IOptions<DataConfiguration> _dataConfiguration;
    private readonly TeamsListExporter _teamsListExporter;
    private readonly FileDownloader _fileDownloader;

    public TeamsList(
        IDbContextFactory<DatabaseContext> databaseContextFactory,
        IOptions<DataConfiguration> dataConfiguration,
        TeamsListExporter teamsListExporter,
        FileDownloader fileDownloader)
    {
        _databaseContextFactory = databaseContextFactory;
        _dataConfiguration = dataConfiguration;
        _teamsListExporter = teamsListExporter;
        _fileDownloader = fileDownloader;
    }

    protected override async Task<ContentState<TeamsListModel>> LoadContent()
    {
        using var databaseContext = _databaseContextFactory.CreateDbContext();
        var attendanceMarks = await databaseContext.AttendanceMarks.ToArrayAsync();

        var viewTeams = _dataConfiguration.Value.Teams
            .OrderBy(team => team.Identifier)
            .Select((team, index) => new TeamsListModel.TeamModel(
                Number: index + 1,
                Identifier: team.Identifier,
                Name: team.Name,
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

        return new TeamsListModel(Teams: viewTeams, SearchFilter: null);
    }

    private async Task ExportDataToExcel()
    {
        var fileName = "Команды участников";
        var fileData = await _teamsListExporter.ExportToExcel();

        await _fileDownloader.DownloadExcelFile(fileName, fileData);
    }

    private bool FilterRecords(TeamsListModel.TeamModel team)
    {
        return string.IsNullOrWhiteSpace(ViewModel.SearchFilter) ||
            team.Name.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            team.Identifier.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase);
    }

    private static object SelectNameField(TeamsListModel.TeamModel team) => team.Name;
    private static object SelectMembersField(TeamsListModel.TeamModel team) => team.MembersQuantity;
    private static object SelectRegistrationsField(TeamsListModel.TeamModel team) => team.RegistrationsQuantity;
    private static object SelectAttendancesField(TeamsListModel.TeamModel team) => team.AttendancesQuantity;
    private static object SelectAbsencesField(TeamsListModel.TeamModel team) => team.AbsencesQuantity;
    private static object SelectSpecialMarksField(TeamsListModel.TeamModel team) => team.SpecialMarksQuantity;
}
