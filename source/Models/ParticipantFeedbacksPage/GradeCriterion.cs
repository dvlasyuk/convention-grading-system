namespace ConventionGradingSystem.Models.ParticipantFeedbacksPage;

/// <summary>
/// Информация о критерии оценивания мероприятия.
/// </summary>
/// <param name="Identifier">Идентификатор критерия.</param>
/// <param name="Name">Человеко-читаемое название критерия.</param>
public record GradeCriterion(string Identifier, string Name);
