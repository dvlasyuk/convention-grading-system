using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using EventGradingSystem.Configuration;
using EventGradingSystem.Database;
using EventGradingSystem.Models.ExpertNotes;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EventGradingSystem.Pages
{
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

        public Event Event { get; set; } = new Event
        {
            EventTypeName = "Неизвестная категория",
            EventName = "Неизвестное мероприятие"
        };

        public List<Note> Notes { get; set; } = new List<Note>();

        public async Task OnGetAsync(int eventTypeId, int eventId)
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

            var notes = await _databaseContext.ExpertNotes
                .Where(item => item.EventTypeId == eventTypeId)
                .Where(item => item.EventId == eventId)
                .ToListAsync();

            Notes = notes
                .OrderBy(item => item.Identifier)
                .Select(item => new Note
                {
                    Identifier = item.Identifier,
                    Content = item.Note
                })
                .ToList();
        }
    }
}
