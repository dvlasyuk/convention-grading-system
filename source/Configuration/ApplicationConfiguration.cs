using ConventionGradingSystem.Configuration.Models;

namespace ConventionGradingSystem.Configuration;

public class ApplicationConfiguration
{
    public ICollection<Contest> Contests { get; } = new List<Contest>();
    public ICollection<Team> Teams { get; } = new List<Team>();
}
