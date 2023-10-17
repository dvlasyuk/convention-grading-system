namespace ConventionGradingSystem.Database.Entities;

/// <summary>
/// Отметка о посещении участником мероприятия в рамках конкурса мероприятий.
/// </summary>
public class AttendanceMark
{
    /// <summary>
    /// Идентификатор участника, посетившего мероприятие.
    /// </summary>
    public required string ParticipantId { get; set; }

    /// <summary>
    /// Идентификатор посещённого мероприятия.
    /// </summary>
    public required string EventId { get; set; }

    /// <summary>
    /// Специальная отметка участника от организаторов мероприятия.
    /// </summary>
    public required bool SpecialMark { get; set; }
}
