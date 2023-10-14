using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using EventGradingSystem.Configuration;
using EventGradingSystem.Database;
using EventGradingSystem.Models.Index;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EventGradingSystem.Pages
{
    [Authorize(Roles = "Adminstrator")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationConfiguration _configuration;
        private readonly DatabaseContext _databaseContext;

        public IndexModel(
            [NotNull] IOptionsSnapshot<ApplicationConfiguration> configuration,
            [NotNull] DatabaseContext databaseContext)
        {
            _configuration = configuration.Value;
            _databaseContext = databaseContext;
        }

        public List<EventType> EventTypes { get; set; } = new List<EventType>();
        public List<Team> Teams { get; set; } = new List<Team>();

        public async Task OnGetAsync()
        {
            EventTypes = _configuration.EventTypes
                .OrderBy(item => item.Identifier)
                .Select(item => new EventType
                {
                    Identifier = item.Identifier,
                    Name = item.Name,
                    EventsQuantity = item.Events.Count
                })
                .ToList();

            var eventParticipants = _configuration.EventTypes
                .SelectMany(item => item.Events)
                .SelectMany(item => item.Participants ?? new List<string>())
                .ToList();

            var participationMarks = await _databaseContext.ParticipationMarks.ToListAsync();
            var specialMarks = await _databaseContext.SpecialMarks.ToListAsync();

            Teams = _configuration.Participants
                .GroupBy(participant => participant.Team)
                .Select(group => new Team
                {
                    Name = group.Key,
                    ParticipantsQuantity = group.Count(),
                    ParticipationRegistrationsQuantity = group
                        .SelectMany(item => eventParticipants
                            .Where(identifier => identifier == item.Identifier))
                        .Count(),
                    ParticipationMarksQuantity = group
                        .SelectMany(item => participationMarks
                            .Where(mark => mark.ParticipantId == item.Identifier))
                        .Count(),
                    SpecialMarksQuantity = group
                        .SelectMany(item => specialMarks
                            .Where(mark => mark.ParticipantId == item.Identifier))
                        .Count()
                })
                .OrderBy(team => team.Name.Length)
                .ThenBy(team => team.Name)
                .ToList();
        }
    }
}
