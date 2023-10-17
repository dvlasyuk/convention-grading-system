namespace ConventionGradingSystem.Host.Models.ParticipantFeedbackForm;

/// <summary>
/// Модель данных формы приложения для сбора отзывов участников о мероприятии в рамках конкурса мероприятий.
/// </summary>
/// <param name="Grades">Информация об оставленных участником оценках.</param>
/// <param name="Note">Дополнительный комментарий, оставленный участником.</param>
public record FormModel(
    IReadOnlyCollection<Grade> Grades,
    string Note);
