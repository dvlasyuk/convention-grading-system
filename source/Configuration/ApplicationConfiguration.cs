using ConventionGradingSystem.Configuration.Models;

namespace ConventionGradingSystem.Configuration;

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
    /// Конфигурация команд участников.
    /// </summary>
    public ICollection<Team> Teams { get; } = new List<Team>();
}
