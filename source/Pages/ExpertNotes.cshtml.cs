using System.Data;
using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Configuration;
using ConventionGradingSystem.Database;
using ConventionGradingSystem.Models.ExpertNotes;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Pages;

[Authorize(Roles = "Adminstrator")]
public class ExpertNotesModel : PageModel
{
    private readonly ApplicationConfiguration _configuration;
    private readonly DatabaseContext _databaseContext;

    public ExpertNotesModel(
        [NotNull] IOptionsSnapshot<ApplicationConfiguration> configuration,
        [NotNull] DatabaseContext databaseContext)
    {
        _configuration = configuration.Value;
        _databaseContext = databaseContext;
    }

    public ViewModel ViewModel { get; set; } = new ViewModel(
        EventTypeName: "Неизвестная категория",
        EventName: "Неизвестное мероприятие",
        Notes: new List<Note>());

    public async Task OnGetAsync(int eventTypeId, int eventId)
    {
        var eventType = _configuration.EventTypes.FirstOrDefault(item => item.Identifier == eventTypeId);
        if (eventType == null)
        {
            return;
        }

        ViewModel = ViewModel with { EventTypeName = eventType.Name };

        var @event = eventType.Events.FirstOrDefault(item => item.Identifier == eventId);
        if (@event == null)
        {
            return;
        }

        ViewModel = ViewModel with { EventName = @event.Name };

        var notes = await _databaseContext.ExpertNotes
            .Where(item => item.EventTypeId == eventTypeId)
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
