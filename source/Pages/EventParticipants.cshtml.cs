using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Configuration;
using ConventionGradingSystem.Database;
using ConventionGradingSystem.Database.Entities;
using ConventionGradingSystem.Models.EventParticipants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Pages;

[Authorize(Roles = "Adminstrator,Organizer")]
public class EventParticipantsModel : PageModel
{
    private readonly ApplicationConfiguration _configuration;
    private readonly DatabaseContext _databaseContext;

    public EventParticipantsModel(
        [NotNull] IOptionsSnapshot<ApplicationConfiguration> configuration,
        [NotNull] DatabaseContext databaseContext)
    {
        _configuration = configuration.Value;
        _databaseContext = databaseContext;
    }

    public Event Event { get; set; } = new Event
    {
        EventTypeName = "Неизвестная категория",
        EventName = "Неизвестное мероприятие"
    };

    public List<Participant> Participants { get; set; } = new List<Participant>();

    [BindProperty]
    public List<string> ParticipationMarks { get; set; }

    [BindProperty]
    public List<string> SpecialMarks { get; set; }

    public async Task OnGetAsync(int eventTypeId, int eventId)
    {
        await InitializeModel(eventTypeId, eventId);
    }

    public async Task OnPostAsync(int eventTypeId, int eventId)
    {
        var participationMarks = await _databaseContext.ParticipationMarks
            .Where(item => item.EventTypeId == eventTypeId)
            .Where(item => item.EventId == eventId)
            .ToListAsync();

        _databaseContext.ParticipationMarks.AddRange(ParticipationMarks
            .Where(participantId => !participationMarks
                .Any(mark => mark.ParticipantId == participantId))
            .Select(participantId => new ParticipationMark
            {
                ParticipantId = participantId,
                EventTypeId = eventTypeId,
                EventId = eventId
            })
            .ToList());

        _databaseContext.ParticipationMarks.RemoveRange(participationMarks
            .Where(mark => !ParticipationMarks
                .Any(participantId => participantId == mark.ParticipantId))
            .ToList());

        var specialMarks = await _databaseContext.SpecialMarks
            .Where(item => item.EventTypeId == eventTypeId)
            .Where(item => item.EventId == eventId)
            .ToListAsync();

        _databaseContext.SpecialMarks.AddRange(SpecialMarks
            .Where(participantId => !specialMarks
                .Any(mark => mark.ParticipantId == participantId))
            .Select(participantId => new SpecialMark
            {
                ParticipantId = participantId,
                EventTypeId = eventTypeId,
                EventId = eventId
            })
            .ToList());

        _databaseContext.SpecialMarks.RemoveRange(specialMarks
            .Where(mark => !SpecialMarks
                .Any(participantId => participantId == mark.ParticipantId))
            .ToList());

        await _databaseContext.SaveChangesAsync();
        await InitializeModel(eventTypeId, eventId);
    }

    private async Task InitializeModel(int eventTypeId, int eventId)
    {
        var eventType = _configuration.EventTypes.FirstOrDefault(item => item.Identifier == eventTypeId);
        if (eventType == null)
        {
            return;
        }

        Event.EventTypeName = eventType.Name;

        var @event = eventType.Events.FirstOrDefault(item => item.Identifier == eventId);
        if (@event == null)
        {
            return;
        }

        Event.EventName = @event.Name;

        var participationMarks = await _databaseContext.ParticipationMarks
            .Where(item => item.EventTypeId == eventTypeId)
            .Where(item => item.EventId == eventId)
            .Select(item => item.ParticipantId)
            .ToListAsync();

        var specialMarks = await _databaseContext.SpecialMarks
            .Where(item => item.EventTypeId == eventTypeId)
            .Where(item => item.EventId == eventId)
            .Select(item => item.ParticipantId)
            .ToListAsync();

        Participants = @event.Participants
            .Select(identifier => _configuration.Participants
                .FirstOrDefault(participant => participant.Identifier == identifier))
            .Select(participant => new Participant
            {
                Identifier = participant.Identifier,
                Name = participant.Name,
                Brigade = participant.Brigade,
                Team = participant.Team,
                ParticipitionMark = participationMarks.Contains(participant.Identifier),
                SpecialMark = specialMarks.Contains(participant.Identifier)
            })
            .ToList();
    }
}
