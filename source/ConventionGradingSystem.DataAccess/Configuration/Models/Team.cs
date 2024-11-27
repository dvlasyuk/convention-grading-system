namespace ConventionGradingSystem.DataAccess.Configuration.Models;

/// <summary>
/// Конфигурация команды участников.
/// </summary>
public class Team
{
    /// <summary>
    /// Идентификатор команды.
    /// </summary>
    /// <remarks>Значение не должно быть пустым и его длина не должна превышать 50 символов. Значение
    /// должно быть уникальным для всех сконфигурированных команд.</remarks>
    public string Identifier { get; set; } = string.Empty;

    /// <summary>
    /// Человеко-читаемое название команды.
    /// </summary>
    /// <remarks>Значение не должно быть пустым и его длина не должна превышать 100 символов.</remarks>
    public string Name { get; set; } = "Неизвестная команда";
}
