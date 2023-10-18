namespace ConventionGradingSystem.DataAccess.Configuration.Models;

/// <summary>
/// Конфигурация критерия оценивания в рамках конкурса мероприятий.
/// </summary>
public class GradeCriterion
{
    /// <summary>
    /// Идентификатор критерия.
    /// </summary>
    /// <remarks>Значение не должно быть пустым и его длина не должна превышать 50 символов. Значение
    /// должно быть уникальным для всех сконфигурированных критериев соответствующего типа (критериев
    /// оценивания экспертами или критериев оценивания участниками).</remarks>
    public string Identifier { get; set; } = string.Empty;

    /// <summary>
    /// Человеко-читаемое название критерия.
    /// </summary>
    /// <remarks>Значение не должно быть пустым и его длина не должна превышать 100 символов.</remarks>
    public string Name { get; set; } = "Неизвестный критерий";

    /// <summary>
    /// Человеко-читаемое описание критерия.
    /// </summary>
    /// <remarks>Значение не должно быть пустым и его длина не должна превышать 1000 символов.</remarks>
    public string Description { get; set; } = "Неизвестный критерий";

    /// <summary>
    /// Минимальное значение оценки для критерия.
    /// </summary>
    /// <remarks>Значение должно быть положительны и строго меньше максимального значения оценки.</remarks>
    public int MinimalGrade { get; set; } = int.MinValue;

    /// <summary>
    /// Максимальное значение оценки для критерия.
    /// </summary>
    /// <remarks>Значение должно быть положительным и строго больше минимального значения оценки.</remarks>
    public int MaximalGrade { get; set; } = int.MinValue;
}
