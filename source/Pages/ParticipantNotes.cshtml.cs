using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Configuration;
using ConventionGradingSystem.Database;
using ConventionGradingSystem.Models.ParticipantNotes;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Pages;

[Authorize(Roles = "Adminstrator")]
public class ParticipantNotesModel : PageModel
{
    private readonly ApplicationConfiguration _configuration;
    private readonly DatabaseContext _databaseContext;

    public ParticipantNotesModel(
        [NotNull] IOptionsSnapshot<ApplicationConfiguration> configuration,
        [NotNull] DatabaseContext databaseContext)
    {
        _configuration = configuration.Value;
        _databaseContext = databaseContext;
    }

    public ViewModel ViewModel { get; private set; } = new ViewModel(
        ContestName: "Неизвестный конкурс",
        EventName: "Неизвестное мероприятие",
        Notes: new List<Note>());

    public async Task OnGetAsync(string contestId, string eventId)
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

        var notes = await _databaseContext.ParticipantNotes
            .Where(item => item.ContestId == contestId)
            .Where(item => item.EventId == eventId)
            .ToListAsync();

        ViewModel = ViewModel with
        {
            Notes = notes
                .OrderBy(item => item.Identifier)
                .Select(item => new Note(
                    Identifier: item.Identifier,
                    Content: item.Note))
                .ToList()
        };
    }
}
