using ConventionGradingSystem.DataAccess.Configuration.Models;

namespace ConventionGradingSystem.DataAccess.Configuration;

public class ApplicationConfiguration
{
    public string ConventionName { get; set; } = "Неизвестный слёт";

    public ICollection<Contest> Contests { get; } = [];

    public ICollection<Voting> Votings { get; } = [];

    public ICollection<Team> Teams { get; } = [];

    public ICollection<Brigade> Brigades { get; } = [];

    public ICollection<Participant> Participants { get; } = [];

    public ICollection<Expert> Experts { get; } = [];
}
