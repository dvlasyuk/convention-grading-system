namespace ConventionGradingSystem.Host.Models.ExpertFeedbacksPage;

/// <summary>
/// Модель представления страницы приложения со списком отзывов экспертов о мероприятии в рамках конкурса мероприятий.
/// </summary>
/// <param name="ContestName">Название конкурса мероприятий.</param>
/// <param name="EventName">Название мероприятия.</param>
/// <param name="Criterions">Критерии оценивания мероприятия.</param>
/// <param name="Feedbacks">Информация об отзывах, оставленных экспертами.</param>
public record ViewModel(
    string ContestName,
    string EventName,
    IReadOnlyList<GradeCriterion> Criterions,
    IReadOnlyList<Feedback> Feedbacks);
