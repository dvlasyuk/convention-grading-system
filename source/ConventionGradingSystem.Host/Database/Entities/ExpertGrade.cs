namespace ConventionGradingSystem.Host.Database.Entities;

/// <summary>
/// Оценка мероприятия в рамках конкурса мероприятий, выставленная экспертом.
/// </summary>
public class ExpertGrade
{
    /// <summary>
    /// Идентификатор отзыва, в рамках которого выставлена оценка.
    /// </summary>
    public Guid FeedbackId { get; set; }

    /// <summary>
    /// Идентификатор критерия оценивания, в соответствии с которым выставлена оценка.
    /// </summary>
    public required string CriterionId { get; set; }

    /// <summary>
    /// Значение выставленной оценки.
    /// </summary>
    public required int GradeValue { get; set; }

    /// <summary>
    /// Отзыв, в рамках которого выставлена оценка.
    /// </summary>
    public ExpertFeedback? Feedback { get; set; }
}
