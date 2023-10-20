namespace ConventionGradingSystem.DataAccess.Configuration.Models;

/// <summary>
/// Конфигурация эксперта.
/// </summary>
public class Expert
{
    /// <summary>
    /// Идентификатор эксперта.
    /// </summary>
    /// <remarks>Значение не должно быть пустым и его длина не должна превышать 50 символов. Значение
    /// должно быть уникальным для всех сконфигурированных экспертов.</remarks>
    public string Identifier { get; set; } = string.Empty;

    /// <summary>
    /// Полное имя эксперта.
    /// </summary>
    /// <remarks>Значение не должно быть пустым и его длина не должна превышать 100 символов.</remarks>
    public string Name { get; set; } = "Неизвестное имя";
}
