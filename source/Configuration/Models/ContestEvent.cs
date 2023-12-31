namespace ConventionGradingSystem.Configuration.Models;

/// <summary>
/// Конфигурация мероприятия, проводимого в рамках конкурса мероприятий.
/// </summary>
public class ContestEvent
{
    /// <summary>
    /// Идентификатор мероприятия.
    /// </summary>
    /// <remarks>Значение не должно быть пустым и его длина не должна превышать 50 символов. Значение
    /// должно быть уникальным для всех сконфигурированных мероприятий.</remarks>
    public string Identifier { get; set; } = string.Empty;

    /// <summary>
    /// Человеко-читаемое название мероприятия.
    /// </summary>
    /// <remarks>Значение не должно быть пустым и его длина не должна превышать 100 символов.</remarks>
    public string Name { get; set; } = "Неизвестное мероприятие";

    /// <summary>
    /// Идентификаторы участников, зарегистрированных для участия в мероприятии.
    /// </summary>
    /// <remarks>Количество сконфигурированных участников не может быть нулевым и не должно превышать
    /// 100 человек. Каждое значение должно быть уникальным в рамках мероприятия.</remarks>
    public ICollection<string> Participants { get; } = new List<string>();
}
