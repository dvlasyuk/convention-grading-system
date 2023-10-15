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

    public async Task OnGetAsync(int contestId, int eventId) =>
        await InitializeModel(contestId, eventId);

    public async Task OnPostAsync(int contestId, int eventId)
    {
        if (FormModel == null)
        {
            throw new InvalidOperationException("Модель формы должна быть заполнена при выполнении POST-запроса");
        }

        var participationMarks = await _databaseContext.ParticipationMarks
            .Where(item => item.ContestId == contestId)
            .Where(item => item.EventId == eventId)
            .ToListAsync();

        _databaseContext.ParticipationMarks.AddRange(FormModel.ParticipationMarks
            .Where(participantId => !participationMarks
                .Any(mark => mark.ParticipantId == participantId))
            .Select(participantId => new ParticipationMark
            {
                ParticipantId = participantId,
                ContestId = contestId,
                EventId = eventId
            })
            .ToList());

        _databaseContext.ParticipationMarks.RemoveRange(participationMarks
            .Where(mark => !FormModel.ParticipationMarks
                .Any(participantId => participantId == mark.ParticipantId))
            .ToList());

        var specialMarks = await _databaseContext.SpecialMarks
            .Where(item => item.ContestId == contestId)
            .Where(item => item.EventId == eventId)
            .ToListAsync();

        _databaseContext.SpecialMarks.AddRange(FormModel.SpecialMarks
            .Where(participantId => !specialMarks
                .Any(mark => mark.ParticipantId == participantId))
            .Select(participantId => new SpecialMark
            {
                ParticipantId = participantId,
                ContestId = contestId,
                EventId = eventId
            })
            .ToList());

        _databaseContext.SpecialMarks.RemoveRange(specialMarks
            .Where(mark => !FormModel.SpecialMarks
                .Any(participantId => participantId == mark.ParticipantId))
            .ToList());

        await _databaseContext.SaveChangesAsync();
        await InitializeModel(contestId, eventId);
    }

    private async Task InitializeModel(int contestId, int eventId)
    {
        var contest = _configuration.Contests.FirstOrDefault(item => item.Identifier == contestId);
        if (contest == null)
        {
            return;
        }

        ViewModel = ViewModel with { ContestName = contest.Name };

        var contestEvent = contest.Events.FirstOrDefault(item => item.Identifier == eventId);
        if (contestEvent == null)
        {
            return;
        }

        ViewModel = ViewModel with { EventName = contestEvent.Name };

        var participationMarks = await _databaseContext.ParticipationMarks
            .Where(item => item.ContestId == contestId)
            .Where(item => item.EventId == eventId)
            .Select(item => item.ParticipantId)
            .ToListAsync();

        var specialMarks = await _databaseContext.SpecialMarks
            .Where(item => item.ContestId == contestId)
            .Where(item => item.EventId == eventId)
            .Select(item => item.ParticipantId)
            .ToListAsync();

        ViewModel = ViewModel with
        {
            Participants = contestEvent.Participants
                .Select(identifier => _configuration.Participants
                    .First(participant => participant.Identifier == identifier))
                .Select(participant => new Participant(
                    Identifier: participant.Identifier,
                    Name: participant.Name,
                    Brigade: participant.Brigade,
                    Team: participant.Team,
                    ParticipitionMark: participationMarks.Contains(participant.Identifier),
                    SpecialMark: specialMarks.Contains(participant.Identifier)))
                .ToList()
        };
    }
}
