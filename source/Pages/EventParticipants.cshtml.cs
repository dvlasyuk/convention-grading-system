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

    public ViewModel ViewModel { get; private set; } = new ViewModel(
        ContestName: "Неизвестный конкурс",
        EventName: "Неизвестное мероприятие",
        Participants: new List<Participant>());

    [BindProperty]
    public FormModel? FormModel { get; set; }

    public async Task OnGetAsync(string eventId) =>
        await InitializeModel(eventId);

    public async Task OnPostAsync(string eventId)
    {
        if (FormModel == null)
        {
            throw new InvalidOperationException("Модель формы должна быть заполнена при выполнении POST-запроса");
        }

        var configuredParticipationMarks = FormModel.ParticipationMarks ?? new List<string>();
        var savedParticipationMarks = await _databaseContext.ParticipationMarks
            .Where(item => item.EventId == eventId)
            .ToListAsync();

        _databaseContext.ParticipationMarks.AddRange(configuredParticipationMarks
            .Where(participantId => !savedParticipationMarks
                .Any(mark => mark.ParticipantId == participantId))
            .Select(participantId => new ParticipationMark
            {
                ParticipantId = participantId,
                EventId = eventId
            })
            .ToList());

        _databaseContext.ParticipationMarks.RemoveRange(savedParticipationMarks
            .Where(mark => !configuredParticipationMarks
                .Any(participantId => participantId == mark.ParticipantId))
            .ToList());

        var configuredSpecialMarks = FormModel.SpecialMarks ?? new List<string>();
        var savedSpecialMarks = await _databaseContext.SpecialMarks
            .Where(item => item.EventId == eventId)
            .ToListAsync();

        _databaseContext.SpecialMarks.AddRange(configuredSpecialMarks
            .Where(participantId => !savedSpecialMarks
                .Any(mark => mark.ParticipantId == participantId))
            .Select(participantId => new SpecialMark
            {
                ParticipantId = participantId,
                EventId = eventId
            })
            .ToList());

        _databaseContext.SpecialMarks.RemoveRange(savedSpecialMarks
            .Where(mark => !configuredSpecialMarks
                .Any(participantId => participantId == mark.ParticipantId))
            .ToList());

        await _databaseContext.SaveChangesAsync();
        await InitializeModel(eventId);
    }

    private async Task InitializeModel(string eventId)
    {
        var contestEvent = _configuration.Contests
            .SelectMany(item => item.Events)
            .FirstOrDefault(item => item.Identifier == eventId);

        if (contestEvent == null)
        {
            return;
        }

        var contest = _configuration.Contests.First(item => item.Events.Contains(contestEvent));
        ViewModel = ViewModel with
        {
            ContestName = contest.Name,
            EventName = contestEvent.Name
        };

        var participationMarks = await _databaseContext.ParticipationMarks
            .Where(item => item.EventId == eventId)
            .Select(item => item.ParticipantId)
            .ToListAsync();

        var specialMarks = await _databaseContext.SpecialMarks
            .Where(item => item.EventId == eventId)
            .Select(item => item.ParticipantId)
            .ToListAsync();

        ViewModel = ViewModel with
        {
            Participants = contestEvent.Participants
                .Select(identifier => _configuration.Teams
                    .SelectMany(team => team.Members)
                    .First(member => member.Identifier == identifier))
                .Select(participant => new Participant(
                    Identifier: participant.Identifier,
                    Name: participant.Name,
                    Brigade: participant.Brigade,
                    ParticipitionMark: participationMarks.Contains(participant.Identifier),
                    SpecialMark: specialMarks.Contains(participant.Identifier)))
                .ToList()
        };
    }
}
