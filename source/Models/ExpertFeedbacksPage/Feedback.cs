namespace ConventionGradingSystem.Models.ExpertFeedbacksPage;

/// <summary>
/// Отзыв о мероприятии, оставленный экспертом.
/// </summary>
/// <param name="Grades">Выставленные экспертом оценки. Ключами словаря выступают идентификаторы
/// критериев оценивания, а значениями - значения соответствующих оценок.</param>
/// <param name="Note">Дополнительный комментарий, оставленный экспертом.</param>
public record Feedback(IReadOnlyDictionary<string, int> Grades, string? Note);
