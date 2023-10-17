using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Configuration;
using ConventionGradingSystem.Database;
using ConventionGradingSystem.Database.Entities;
using ConventionGradingSystem.Models.EventAttendanceForm;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Pages;

[Authorize(Roles = "Adminstrator,Organizer")]
public class EventAttendanceFormModel : PageModel
{
    private readonly ApplicationConfiguration _configuration;
    private readonly DatabaseContext _databaseContext;

    public EventAttendanceFormModel(
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

        var configuredAttendanceMarks = FormModel.AttendanceMarks ?? new List<string>();
        var configuredSpecialMarks = FormModel.SpecialMarks ?? new List<string>();

        var savedMarks = await _databaseContext.AttendanceMarks
            .Where(item => item.EventId == eventId)
            .ToListAsync();

        _databaseContext.AttendanceMarks.AddRange(configuredAttendanceMarks
            .Where(participantId => !savedMarks
                .Any(mark => mark.ParticipantId == participantId))
            .Select(participantId => new AttendanceMark
            {
                ParticipantId = participantId,
                EventId = eventId,
                SpecialMark = configuredSpecialMarks
                    .Any(mark => mark == participantId)
            })
            .ToList());

        _databaseContext.AttendanceMarks.RemoveRange(savedMarks
            .Where(mark => !configuredAttendanceMarks
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

        var savedAttendanceMarks = await _databaseContext.AttendanceMarks
            .Where(item => item.EventId == eventId)
            .ToListAsync();

        var attendanceMarks = savedAttendanceMarks
            .Select(item => item.ParticipantId)
            .ToList();

        var specialMarks = savedAttendanceMarks
            .Where(item => item.SpecialMark)
            .Select(item => item.ParticipantId)
            .ToList();

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
                    AttendanceMark: attendanceMarks.Contains(participant.Identifier),
                    SpecialMark: specialMarks.Contains(participant.Identifier)))
                .ToList()
        };
    }
}
