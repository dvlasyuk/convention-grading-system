using ConventionGradingSystem.DataAccess.Configuration.Models;

namespace ConventionGradingSystem.DataAccess.Configuration;

/// <summary>
/// Конфигурационные данные приложения.
/// </summary>
public class ApplicationConfiguration
{
    /// <summary>
    /// Конфигурация конкурсов мероприятий.
    /// </summary>
    public ICollection<Contest> Contests { get; } = new List<Contest>();

    /// <summary>
    /// Конфигурация зрительских голосований.
    /// </summary>
    public ICollection<Voting> Votings { get; } = new List<Voting>();

    /// <summary>
    /// Конфигурация команд участников.
    /// </summary>
    public ICollection<Team> Teams { get; } = new List<Team>();
}
