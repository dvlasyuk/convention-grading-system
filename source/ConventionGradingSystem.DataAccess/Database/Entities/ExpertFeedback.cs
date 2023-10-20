namespace ConventionGradingSystem.DataAccess.Database.Entities;

/// <summary>
/// Отзыв эксперта на мероприятие в рамках конкурса мероприятий.
/// </summary>
public class ExpertFeedback
{
    /// <summary>
    /// Идентификатор отзыва.
    /// </summary>
    public Guid Identifier { get; set; }

    /// <summary>
    /// Идентификатор мероприятия, для которого оставлен отзыв.
    /// </summary>
    public required string EventId { get; set; }

    /// <summary>
    /// Идентификатор эксперта, который оставил отзыв.
    /// </summary>
    public required string ExpertId { get; set; }

    /// <summary>
    /// Дополнительный комментарий, оставленный экспертом.
    /// </summary>
    public required string? Note { get; set; }

    /// <summary>
    /// Оценки, выставленные мероприятию экспертом.
    /// </summary>
    public ICollection<ExpertGrade> Grades { get; } = new List<ExpertGrade>();
}
