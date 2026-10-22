using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.Host.Components.Administration.Participants.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Administration.Participants.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class TeamsList
{
    private readonly DatabaseContext _databaseContext;
    private readonly IOptions<ApplicationConfiguration> _configuration;

    private readonly TeamsListModel _viewModel = new();

    public TeamsList(
        DatabaseContext databaseContext,
        IOptions<ApplicationConfiguration> configuration)
    {
        _databaseContext = databaseContext;
        _configuration = configuration;
    }

    protected override async Task OnInitializedAsync()
    {
        var attendanceMarks = await _databaseContext.AttendanceMarks.ToListAsync();
        _viewModel.Teams = [.. _configuration.Value.Teams
            .OrderBy(team => team.Identifier)
            .Select((team, index) => new TeamsListModel.TeamModel(
                Number: index + 1,
                Identifier: team.Identifier,
                Name: team.Name,
                MembersQuantity: _configuration.Value.Participants
                    .Count(participant => participant.Team == team.Identifier),
                RegistrationsQuantity: _configuration.Value.Participants
                    .Where(participant => participant.Team == team.Identifier)
                    .SelectMany(participant => _configuration.Value.Contests
                        .SelectMany(contest => contest.Events)
                        .SelectMany(@event => @event.Participants)
                        .Where(identifier => identifier == participant.Identifier))
                    .Count(),
                AttendancesQuantity: _configuration.Value.Participants
                    .Where(participant => participant.Team == team.Identifier)
                    .SelectMany(participant => attendanceMarks
                        .Where(mark => mark.ParticipantId == participant.Identifier))
                    .Count(),
                AbsencesQuantity:
                    _configuration.Value.Participants
                        .Where(participant => participant.Team == team.Identifier)
                        .SelectMany(participant => _configuration.Value.Contests
                            .SelectMany(contest => contest.Events)
                            .SelectMany(@event => @event.Participants)
                            .Where(identifier => identifier == participant.Identifier))
                        .Count() -
                    _configuration.Value.Participants
                        .Where(participant => participant.Team == team.Identifier)
                        .SelectMany(participant => attendanceMarks
                            .Where(mark => mark.ParticipantId == participant.Identifier))
                        .Count(),
                SpecialMarksQuantity: _configuration.Value.Participants
                    .Where(participant => participant.Team == team.Identifier)
                    .SelectMany(participant => attendanceMarks
                        .Where(mark => mark.ParticipantId == participant.Identifier))
                    .Count(mark => mark.SpecialMark)))];
    }

    private bool FilterRecords(TeamsListModel.TeamModel team)
    {
        return string.IsNullOrWhiteSpace(_viewModel.SearchFilter) ||
            team.Name.Contains(_viewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            team.Identifier.Contains(_viewModel.SearchFilter, StringComparison.OrdinalIgnoreCase);
    }

    private static object SelectNameField(TeamsListModel.TeamModel team) => team.Name;
    private static object SelectMembersField(TeamsListModel.TeamModel team) => team.MembersQuantity;
    private static object SelectRegistrationsField(TeamsListModel.TeamModel team) => team.RegistrationsQuantity;
    private static object SelectAttendancesField(TeamsListModel.TeamModel team) => team.AttendancesQuantity;
    private static object SelectAbsencesField(TeamsListModel.TeamModel team) => team.AbsencesQuantity;
    private static object SelectSpecialMarksField(TeamsListModel.TeamModel team) => team.SpecialMarksQuantity;
}
