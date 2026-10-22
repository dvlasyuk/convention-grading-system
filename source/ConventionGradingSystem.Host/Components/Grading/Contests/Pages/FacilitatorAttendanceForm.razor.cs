using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.DataAccess.Database.Entities;
using ConventionGradingSystem.Host.Components.Grading.Contests.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Grading.Contests.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class FacilitatorAttendanceForm
{
    private readonly DatabaseContext _databaseContext;
    private readonly IOptions<ApplicationConfiguration> _configuration;
    private readonly NavigationManager _navigationManager;

    private readonly FacilitatorAttendanceFormModel _viewModel = new();

    [Parameter]
    public string? EventId { get; set; }

    public FacilitatorAttendanceForm(
        DatabaseContext databaseContext,
        IOptions<ApplicationConfiguration> configuration,
        NavigationManager navigationManager)
    {
        _databaseContext = databaseContext;
        _configuration = configuration;
        _navigationManager = navigationManager;
    }

    protected override async Task OnInitializedAsync()
    {
        var contestEvent = _configuration.Value.Contests
            .SelectMany(@event => @event.Events)
            .FirstOrDefault(@event => @event.Identifier == EventId);

        if (contestEvent == null)
        {
            return;
        }

        var contest = _configuration.Value.Contests.First(item => item.Events.Contains(contestEvent));
        var participants = _configuration.Value.Participants.ToDictionary(item => item.Identifier);
        var brigades = _configuration.Value.Brigades.ToDictionary(brigade => brigade.Identifier);
        var teams = _configuration.Value.Teams.ToDictionary(team => team.Identifier);

        _viewModel.Event = new FacilitatorAttendanceFormModel.EventModel(
            EventName: contestEvent.Name,
            ContestName: contest.Name,
            AttendanceControl: contest.AttendanceControl,
            Participants: [.. contestEvent.Participants
                .Select(identifier => participants[identifier])
                .Select(participant => new FacilitatorAttendanceFormModel.ParticipantModel(
                    Identifier: participant.Identifier,
                    Name: participant.Name,
                    Brigade: !string.IsNullOrWhiteSpace(participant.Brigade)
                        ? brigades[participant.Brigade].Name
                        : null,
                    Team: teams[participant.Team].Name))]);

        var savedAttendanceMarks = await _databaseContext.AttendanceMarks
            .Where(item => item.EventId == EventId)
            .ToListAsync();

        foreach (var participant in _viewModel.Event.Participants)
        {
            _viewModel.AttendanceMarks.Add(participant.Identifier, savedAttendanceMarks.Any(mark => mark.ParticipantId == participant.Identifier));
            _viewModel.SpecialMarks.Add(participant.Identifier, savedAttendanceMarks.Any(mark =>
                mark.ParticipantId == participant.Identifier &&
                mark.SpecialMark));
        }
    }

    private async Task ProcessForm()
    {
        var savedAttendanceMarks = await _databaseContext.AttendanceMarks
            .Where(item => item.EventId == EventId)
            .ToListAsync();

        foreach (var particiapntId in _viewModel.AttendanceMarks.Keys)
        {
            var savedMark = savedAttendanceMarks.FirstOrDefault(mark => mark.ParticipantId == particiapntId);
            if (savedMark == null)
            {
                if (_viewModel.AttendanceMarks[particiapntId])
                {
                    _databaseContext.AttendanceMarks.Add(new AttendanceMark
                    {
                        ParticipantId = particiapntId,
                        EventId = EventId!,
                        SpecialMark = _viewModel.SpecialMarks[particiapntId]
                    });
                }

                continue;
            }

            if (!_viewModel.AttendanceMarks[particiapntId])
            {
                _databaseContext.AttendanceMarks.Remove(savedMark);
            }

            savedMark.SpecialMark = _viewModel.SpecialMarks[particiapntId];
            _databaseContext.AttendanceMarks.Update(savedMark);
        }

        await _databaseContext.SaveChangesAsync();
        _navigationManager.NavigateTo($"/grading/events/{EventId}/facilitator-attendance", true);
    }

    private bool FilterRecords(FacilitatorAttendanceFormModel.ParticipantModel participant)
    {
        return string.IsNullOrWhiteSpace(_viewModel.SearchFilter) ||
            participant.Name.Contains(_viewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            participant.Team.Contains(_viewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            (participant.Brigade?.Contains(_viewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private static object SelectNameField(FacilitatorAttendanceFormModel.ParticipantModel participant) => participant.Name;
    private static object SelectBrigadeField(FacilitatorAttendanceFormModel.ParticipantModel participant) => participant.Brigade ?? string.Empty;
    private static object SelectTeamField(FacilitatorAttendanceFormModel.ParticipantModel participant) => participant.Team;
}
