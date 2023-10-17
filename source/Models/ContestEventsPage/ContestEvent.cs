namespace ConventionGradingSystem.Models.ContestEventsPage;

/// <summary>
/// Информация о мероприятии в рамках конкурса мероприятий.
/// </summary>
/// <param name="Identifier">Идентификатор мероприятия.</param>
/// <param name="Name">Человеко-читаемое название мероприятия.</param>
/// <param name="ExprertFeedbacksQuantity">Количество отзывов к мероприятию, оставленных экспертами.</param>
/// <param name="ParticipantFeedbacksQuantity">Количество отзывов к мероприятию, оставленных участниками.</param>
/// <param name="ExpertGrades">Усреднённые оценки мероприятия, оставленные экспертами. Ключом словаря
/// выступает идентификатор критерия оценивания, а значением - значение соответствующей оценки.</param>
/// <param name="ParticipantGrades">Усреднённые оценки мероприятия, оставленные участниками. Ключом слваря
/// выступает идентификатор критерия оценивания, а значением - значение соответствующей оценки.</param>
/// <param name="TotalExpertGrade">Итоговая оценка мероприятия, оставленная экспертами.</param>
/// <param name="TotalParticipantGrade">Итоговая оценка мероприятия, оставленная участниками.</param>
/// <param name="TotalGrade">Итоговая оценка мероприятия.</param>
public record ContestEvent(
    string Identifier,
    string Name,
    int ExprertFeedbacksQuantity,
    int ParticipantFeedbacksQuantity,
    IReadOnlyDictionary<string, float> ExpertGrades,
    IReadOnlyDictionary<string, float> ParticipantGrades,
    float TotalExpertGrade,
    float TotalParticipantGrade,
    float TotalGrade);
