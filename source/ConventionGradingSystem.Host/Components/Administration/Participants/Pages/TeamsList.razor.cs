using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Administration.Participants.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class TeamsList
{
    private readonly DatabaseContext _databaseContext;
    private readonly IOptions<ApplicationConfiguration> _configuration;

    private IEnumerable<ParticipantsTeam> _teams = [];
    private string? _searchFilter;

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
        _teams = [.. _configuration.Value.Teams
            .OrderBy(team => team.Identifier)
            .Select((team, index) => new ParticipantsTeam(
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

    private bool FilterRecords(ParticipantsTeam team)
    {
        return string.IsNullOrWhiteSpace(_searchFilter) ||
            team.Name.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase) ||
            team.Identifier.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase);
    }

    private static object SelectNameField(ParticipantsTeam team) => team.Name;
    private static object SelectMembersField(ParticipantsTeam team) => team.MembersQuantity;
    private static object SelectRegistrationsField(ParticipantsTeam team) => team.RegistrationsQuantity;
    private static object SelectAttendancesField(ParticipantsTeam team) => team.AttendancesQuantity;
    private static object SelectAbsencesField(ParticipantsTeam team) => team.AbsencesQuantity;
    private static object SelectSpecialMarksField(ParticipantsTeam team) => team.SpecialMarksQuantity;

    private sealed record ParticipantsTeam(
        int Number,
        string Identifier,
        string Name,
        int MembersQuantity,
        int RegistrationsQuantity,
        int AttendancesQuantity,
        int AbsencesQuantity,
        int SpecialMarksQuantity);
}
