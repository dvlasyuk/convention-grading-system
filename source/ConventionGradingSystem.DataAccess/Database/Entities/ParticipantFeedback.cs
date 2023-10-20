namespace ConventionGradingSystem.DataAccess.Database.Entities;

/// <summary>
/// Отзыв участника на мероприятие в рамках конкурса мероприятий.
/// </summary>
public class ParticipantFeedback
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
    /// Идентификатор пользователя, который оставил отзыв.
    /// </summary>
    public required string ParticipantId { get; set; }

    /// <summary>
    /// Дополнительный комментарий, оставленный участником.
    /// </summary>
    public required string? Note { get; set; }

    /// <summary>
    /// Оценки, выставленные мероприятию участником.
    /// </summary>
    public ICollection<ParticipantGrade> Grades { get; } = new List<ParticipantGrade>();
}
