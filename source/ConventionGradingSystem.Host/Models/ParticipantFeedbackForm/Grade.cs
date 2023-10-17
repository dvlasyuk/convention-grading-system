namespace ConventionGradingSystem.Host.Models.ParticipantFeedbackForm;

/// <summary>
/// Оценка мероприятия, оставленная участником.
/// </summary>
/// <param name="CriterionId">Идентификатор критерия оценивания.</param>
/// <param name="GradeValue">Значение оценки.</param>
public record Grade(string CriterionId, int GradeValue);
