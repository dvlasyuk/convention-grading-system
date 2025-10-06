using ConventionGradingSystem.DataAccess.Configuration.Models;

namespace ConventionGradingSystem.DataAccess.Configuration;

/// <summary>
/// Конфигурационные данные приложения.
/// </summary>
public class ApplicationConfiguration
{
    /// <summary>
    /// Название слёта, для которого работает система.
    /// </summary>
    /// <remarks>Значение не должно быть пустым и его длина не должна превышать 100 символов.</remarks>
    public string ConventionName { get; set; } = "Неизвестный слёт";

    /// <summary>
    /// Конфигурация конкурсов мероприятий.
    /// </summary>
    public ICollection<Contest> Contests { get; } = [];

    /// <summary>
    /// Конфигурация зрительских голосований.
    /// </summary>
    public ICollection<Voting> Votings { get; } = [];

    /// <summary>
    /// Конфигурация команд участников.
    /// </summary>
    public ICollection<Team> Teams { get; } = [];

    /// <summary>
    /// Конфигурация отрядов.
    /// </summary>
    public ICollection<Brigade> Brigades { get; } = [];

    /// <summary>
    /// Конфигурация участников.
    /// </summary>
    public ICollection<Participant> Participants { get; } = [];

    /// <summary>
    /// Конфигурация экспертов.
    /// </summary>
    public ICollection<Expert> Experts { get; } = [];
}
