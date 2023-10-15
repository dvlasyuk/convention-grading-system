using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Configuration;
using ConventionGradingSystem.Database;
using ConventionGradingSystem.Models.Index;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Pages;

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

    public ViewModel ViewModel { get; private set; } = new ViewModel(
        Contests: new List<Contest>(),
        Teams: new List<Team>());

    public async Task OnGetAsync()
    {
        ViewModel = ViewModel with
        {
            Contests = _configuration.Contests
                .OrderBy(item => item.Identifier)
                .Select(item => new Contest(
                    Identifier: item.Identifier,
                    Name: item.Name,
                    EventsQuantity: item.Events.Count))
                .ToList()
        };

        var eventParticipants = _configuration.Contests
            .SelectMany(item => item.Events)
            .SelectMany(item => item.Participants)
            .ToList();

        var participationMarks = await _databaseContext.ParticipationMarks.ToListAsync();
        var specialMarks = await _databaseContext.SpecialMarks.ToListAsync();

        ViewModel = ViewModel with
        {
            Teams = _configuration.Participants
                .GroupBy(participant => participant.Team)
                .Select(group => new Team(
                    Name: group.Key,
                    ParticipantsQuantity: group.Count(),
                    ParticipationRegistrationsQuantity: group
                        .SelectMany(item => eventParticipants
                            .Where(identifier => identifier == item.Identifier))
                        .Count(),
                    ParticipationMarksQuantity: group
                        .SelectMany(item => participationMarks
                            .Where(mark => mark.ParticipantId == item.Identifier))
                        .Count(),
                    SpecialMarksQuantity: group
                        .SelectMany(item => specialMarks
                            .Where(mark => mark.ParticipantId == item.Identifier))
                        .Count()))
                .OrderBy(team => team.Name.Length)
                .ThenBy(team => team.Name)
                .ToList()
        };
    }
}
