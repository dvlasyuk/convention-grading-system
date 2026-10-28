using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.DataAccess.Database.Entities;
using ConventionGradingSystem.Host.Components.Grading.Contests.Models;
using ConventionGradingSystem.Host.Components.Shared.Models;
using ConventionGradingSystem.Host.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Grading.Contests.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class FacilitatorAttendanceForm
{
    private readonly IDbContextFactory<DatabaseContext> _databaseContextFactory;
    private readonly IOptions<DataConfiguration> _dataConfiguration;
    private readonly NavigationManager _navigationManager;

    [Parameter]
    public string? EventId { get; set; }

    public FacilitatorAttendanceForm(
        IDbContextFactory<DatabaseContext> databaseContextFactory,
        IOptions<DataConfiguration> dataConfiguration,
        NavigationManager navigationManager)
    {
        _databaseContextFactory = databaseContextFactory;
        _dataConfiguration = dataConfiguration;
        _navigationManager = navigationManager;
    }

    protected override async Task<ContentState<FacilitatorAttendanceFormModel>> LoadContent()
    {
        var contestEvent = _dataConfiguration.Value.Contests
            .SelectMany(@event => @event.Events)
            .FirstOrDefault(@event => @event.Identifier == EventId);

        if (contestEvent == null)
        {
            return Shared.Models.LoadingError.EventNotFound;
        }

        var contest = _dataConfiguration.Value.Contests.First(item => item.Events.Contains(contestEvent));
        var participants = _dataConfiguration.Value.Participants.ToDictionary(item => item.Identifier);
        var brigades = _dataConfiguration.Value.Brigades.ToDictionary(brigade => brigade.Identifier);
        var teams = _dataConfiguration.Value.Teams.ToDictionary(team => team.Identifier);

        var viewEvent = new FacilitatorAttendanceFormModel.EventModel(
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

        using var databaseContext = _databaseContextFactory.CreateDbContext();
        var savedAttendanceMarks = await databaseContext.AttendanceMarks
            .Where(item => item.EventId == EventId)
            .ToListAsync();

        var viewAttendanceMarks = new Dictionary<string, bool>();
        var viewSpecialMarks = new Dictionary<string, bool>();
        foreach (var participant in viewEvent.Participants)
        {
            viewAttendanceMarks.Add(participant.Identifier, savedAttendanceMarks.Any(mark => mark.ParticipantId == participant.Identifier));
            viewSpecialMarks.Add(participant.Identifier, savedAttendanceMarks.Any(mark =>
                mark.ParticipantId == participant.Identifier &&
                mark.SpecialMark));
        }

        return new FacilitatorAttendanceFormModel(
            Event: viewEvent,
            Form: new FacilitatorAttendanceFormModel.FormModel(
                AttendanceMarks: viewAttendanceMarks,
                SpecialMarks: viewSpecialMarks),
            SearchFilter: null);
    }

    private async Task ProcessForm()
    {
        using var databaseContext = _databaseContextFactory.CreateDbContext();
        var savedAttendanceMarks = await databaseContext.AttendanceMarks
            .Where(item => item.EventId == EventId)
            .ToListAsync();

        foreach (var particiapntId in ViewModel.Form.AttendanceMarks.Keys)
        {
            var savedMark = savedAttendanceMarks.FirstOrDefault(mark => mark.ParticipantId == particiapntId);
            if (savedMark == null)
            {
                if (ViewModel.Form.AttendanceMarks[particiapntId])
                {
                    databaseContext.AttendanceMarks.Add(new AttendanceMark
                    {
                        ParticipantId = particiapntId,
                        EventId = EventId!,
                        SpecialMark = ViewModel.Form.SpecialMarks[particiapntId]
                    });
                }

                continue;
            }

            if (!ViewModel.Form.AttendanceMarks[particiapntId])
            {
                databaseContext.AttendanceMarks.Remove(savedMark);
            }

            savedMark.SpecialMark = ViewModel.Form.SpecialMarks[particiapntId];
            databaseContext.AttendanceMarks.Update(savedMark);
        }

        await databaseContext.SaveChangesAsync();
        _navigationManager.NavigateTo(
            uri: NavigationHelper.Grading.FacilitatorAttendanceForm(EventId!),
            forceLoad: true);
    }

    private bool FilterRecords(FacilitatorAttendanceFormModel.ParticipantModel participant)
    {
        return string.IsNullOrWhiteSpace(ViewModel.SearchFilter) ||
            participant.Name.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            participant.Team.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            (participant.Brigade?.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private static object SelectNameField(FacilitatorAttendanceFormModel.ParticipantModel participant) => participant.Name;
    private static object SelectBrigadeField(FacilitatorAttendanceFormModel.ParticipantModel participant) => participant.Brigade ?? string.Empty;
    private static object SelectTeamField(FacilitatorAttendanceFormModel.ParticipantModel participant) => participant.Team;
}
