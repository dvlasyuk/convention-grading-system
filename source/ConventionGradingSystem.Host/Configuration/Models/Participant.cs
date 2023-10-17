namespace ConventionGradingSystem.Host.Configuration.Models;

/// <summary>
/// Конфигурация участника.
/// </summary>
public class Participant
{
    /// <summary>
    /// Идентификатор участника.
    /// </summary>
    /// <remarks>Значение не должно быть пустым и его длина не должна превышать 50 символов. Значение
    /// должно быть уникальным для всех сконфигурированных участников.</remarks>
    public string Identifier { get; set; } = string.Empty;

    /// <summary>
    /// Полное имя участника.
    /// </summary>
    /// <remarks>Значение не должно быть пустым и его длина не должна превышать 100 символов.</remarks>
    public string Name { get; set; } = "Неизвестное имя";

    /// <summary>
    /// Полное название отряда, к которому относится участник.
    /// </summary>
    /// <remarks>Значение не должно быть пустым и его длина не должна превышать 100 символов.</remarks>
    public string Brigade { get; set; } = "Неизвестный отряд";
}
