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
            Teams = _configuration.Teams
                .Select(team => new Team(
                    Name: team.Name,
                    MembersQuantity: team.Members.Count,
                    MembersRegistrationsQuantity: team.Members
                        .SelectMany(member => eventParticipants
                            .Where(identifier => identifier == member.Identifier))
                        .Count(),
                    MembersMarksQuantity: team.Members
                        .SelectMany(member => participationMarks
                            .Where(mark => mark.ParticipantId == member.Identifier))
                        .Count(),
                    SpecialMarksQuantity: team.Members
                        .SelectMany(member => specialMarks
                            .Where(mark => mark.ParticipantId == member.Identifier))
                        .Count()))
                .OrderBy(team => team.Name.Length)
                .ThenBy(team => team.Name)
                .ToList()
        };
    }
}
