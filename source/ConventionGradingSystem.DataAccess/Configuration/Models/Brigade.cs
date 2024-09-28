namespace ConventionGradingSystem.DataAccess.Configuration.Models;

/// <summary>
/// Конфигурация отряда.
/// </summary>
public class Brigade
{
    /// <summary>
    /// Идентификатор отряда.
    /// </summary>
    /// <remarks>Значение не должно быть пустым и его длина не должна превышать 50 символов. Значение
    /// должно быть уникальным для всех сконфигурированных отрядов.</remarks>
    public string Identifier { get; set; } = string.Empty;

    /// <summary>
    /// Человеко-читаемое название отряда.
    /// </summary>
    /// <remarks>Значение не должно быть пустым и его длина не должна превышать 100 символов.</remarks>
    public string Name { get; set; } = "Неизвестная команда";
}
