namespace ConventionGradingSystem.DataAccess.Configuration.Models;

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
    /// Признак, допустимо ли оценивать мероприятие только участникам, которые зарегистрированы для
    /// участия в нём.
    /// </summary>
    public bool RegisteredGrading { get; set; } = true;

    /// <summary>
    /// Признак, допустимо ли участникам оценивать мероприятие, имеющее прямое отношение к тому же
    /// отряду, что и сами участники.
    /// </summary>
    public bool FriendlyGrading { get; set; } = true;

    /// <summary>
    /// Признак, контролируется ли посещаемость мероприятий в рамках конкурса.
    /// </summary>
    public bool AttendanceControl { get; set; } = true;

    /// <summary>
    /// Критерии оценивания конкурса экспертами.
    /// </summary>
    /// <remarks>Количество сконфигурированных критериев не должно превышать 10 и может быть нулевым,
    /// только если количество сконфигурированных критериев оценивания участниками не нулевое.</remarks>
    public ICollection<GradeCriterion> ExpertCriterions { get; } = [];

    /// <summary>
    /// Критерии оценивания конкурса участниками.
    /// </summary>
    /// <remarks>Количество сконфигурированных критериев не должно превышать 10 и может быть нулевым,
    /// только если количество сконфигурированных критериев оценивания экспертами не нулевое.</remarks>
    public ICollection<GradeCriterion> ParticipantCriterions { get; } = [];

    /// <summary>
    /// Мероприятия, проводимые в рамках конкурса.
    /// </summary>
    /// <remarks>Количество сконфигурированных мероприятий не может быть нулевым и не должно превышать
    /// 100 мероприятий.</remarks>
    public ICollection<ContestEvent> Events { get; } = [];
}
