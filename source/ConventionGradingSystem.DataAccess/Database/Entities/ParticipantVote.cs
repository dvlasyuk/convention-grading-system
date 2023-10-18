namespace ConventionGradingSystem.DataAccess.Database.Entities;

/// <summary>
/// Голос участника в рамках зрительского голосования.
/// </summary>
public class ParticipantVote
{
    /// <summary>
    /// Идентификатор голоса.
    /// </summary>
    public Guid Identifier { get; set; }

    /// <summary>
    /// Идентификатор участника, который оставил голос.
    /// </summary>
    public required string ParticipantId { get; set; }

    /// <summary>
    /// Идентификатор участника голосования, за которого оставлен голос.
    /// </summary>
    public required string VoitingParticipantId { get; set; }

    /// <summary>
    /// Дополнительный комментарий, оставленный участником.
    /// </summary>
    public required string? Note { get; set; }
}
