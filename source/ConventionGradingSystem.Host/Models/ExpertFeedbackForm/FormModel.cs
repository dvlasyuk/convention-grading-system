namespace ConventionGradingSystem.Host.Models.ExpertFeedbackForm;

/// <summary>
/// Модель данных формы приложения для сбора отзывов экспертов о мероприятии в рамках конкурса мероприятий.
/// </summary>
/// <param name="Grades">Информация об оставленных экспертом оценках.</param>
/// <param name="Note">Дополнительный комментарий, оставленный экспертом.</param>
public record FormModel(
    IReadOnlyCollection<Grade> Grades,
    string Note);
