using ConventionGradingSystem.Configuration.Contests;
using ConventionGradingSystem.Configuration.Participants;

namespace ConventionGradingSystem.Configuration;

public class ApplicationConfiguration
{
    public ICollection<Contest> Contests { get; } = new List<Contest>();
    public ICollection<Team> Teams { get; } = new List<Team>();
}
