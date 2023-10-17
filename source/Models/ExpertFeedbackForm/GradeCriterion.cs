namespace ConventionGradingSystem.Models.ExpertFeedbackForm;

/// <summary>
/// Информация о критерии оценивания мероприятия.
/// </summary>
/// <param name="Identifier">Идентификатор критерия.</param>
/// <param name="Name">Человеко-читаемое название критерия.</param>
/// <param name="Description">Человеко-читаемое описание критерия.</param>
/// <param name="MinimalGrade">Минимальное значение оценки.</param>
/// <param name="MaximalGrade">Максимальное значение оценки.</param>
public record GradeCriterion(
    string Identifier,
    string Name,
    string Description,
    int MinimalGrade,
    int MaximalGrade);
