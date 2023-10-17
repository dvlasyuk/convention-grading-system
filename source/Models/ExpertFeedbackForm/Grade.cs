namespace ConventionGradingSystem.Models.ExpertFeedbackForm;

/// <summary>
/// Оценка мероприятия, оставленная экспертом.
/// </summary>
/// <param name="CriterionId">Идентификатор критерия оценивания.</param>
/// <param name="GradeValue">Значение оценки.</param>
public record Grade(string CriterionId, int GradeValue);
