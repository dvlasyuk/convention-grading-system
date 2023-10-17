namespace ConventionGradingSystem.Models.ParticipantFeedbackForm;

/// <summary>
/// Состояние формы приложения для сбора отзывов участников о мероприятии в рамках конкурса мероприятий.
/// </summary>
public enum FormState
{
    /// <summary>
    /// Заданное мероприятие не существует.
    /// </summary>
    NotExisted,

    /// <summary>
    /// Заданное мероприятие не оценено.
    /// </summary>
    NotGraded,

    /// <summary>
    /// Заданное мероприятие оценено только что.
    /// </summary>
    JustGraded,

    /// <summary>
    /// Заданное мероприятие было оценено ранее.
    /// </summary>
    PreviouslyGraded
}
