namespace ConventionGradingSystem.Configuration.Models;

/// <summary>
/// Конфигурация конкурса мероприятий.
/// </summary>
public class Contest
{
    /// <summary>
    /// Идентификатор конкурса.
    /// </summary>
    /// <remarks>Значение не должно быть пустым и его длина не должна превышать 50 символов. Значение
    /// должно быть уникальным для всех сконфигурированных конкурсов.</remarks>
    public string Identifier { get; set; } = string.Empty;

    /// <summary>
    /// Человеко-читаемое название конкурса.
    /// </summary>
    /// <remarks>Значение не должно быть пустым и его длина не должна превышать 100 символов.</remarks>
    public string Name { get; set; } = "Неизвестный конкурс";

    /// <summary>
    /// Критерии оценивания конкурса экспертами.
    /// </summary>
    /// <remarks>Количество сконфигурированных критериев не должно превышать 10 и может быть нулевым,
    /// только если количество сконфигурированных критериев оценивания участниками не нулевое.</remarks>
    public ICollection<GradeCriterion> ExpertCriterions { get; } = new List<GradeCriterion>();

    /// <summary>
    /// Критерии оценивания конкурса участниками.
    /// </summary>
    /// <remarks>Количество сконфигурированных критериев не должно превышать 10 и может быть нулевым,
    /// только если количество сконфигурированных критериев оценивания экспертами не нулевое.</remarks>
    public ICollection<GradeCriterion> ParticipantCriterions { get; } = new List<GradeCriterion>();

    /// <summary>
    /// Мероприятия, проводимые в рамках конкурса.
    /// </summary>
    /// <remarks>Количество сконфигурированных мероприятий не может быть нулевым и не должно превышать
    /// 100 мероприятий.</remarks>
    public ICollection<ContestEvent> Events { get; } = new List<ContestEvent>();
}
