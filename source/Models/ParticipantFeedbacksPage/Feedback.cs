namespace ConventionGradingSystem.Models.ParticipantFeedbacksPage;

/// <summary>
/// Отзыв о мероприятии, оставленный участником.
/// </summary>
/// <param name="Grades">Выставленные участником оценки. Ключами словаря выступают идентификаторы
/// критериев оценивания, а значениями - значения соответствующих оценок.</param>
/// <param name="Note">Дополнительный комментарий, оставленный участником.</param>
public record Feedback(IReadOnlyDictionary<string, int> Grades, string? Note);
