namespace ConventionGradingSystem.Models.ParticipantFeedbackForm;

/// <summary>
/// Модель представления формы приложения для сбора отзывов участников о мероприятии в рамках конкурса мероприятий.
/// </summary>
/// <param name="ContestName">Название конкурса мероприятий.</param>
/// <param name="EventName">Название мероприятия.</param>
/// <param name="Criterions">Критерии оценивания мероприятия.</param>
public record ViewModel(
    string ContestName,
    string EventName,
    IReadOnlyList<GradeCriterion> Criterions);
