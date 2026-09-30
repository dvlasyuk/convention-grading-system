namespace ConventionGradingSystem.DataAccess.Database.Entities;

public class ParticipantVote
{
    public Guid Identifier { get; set; }

    public required string ParticipantId { get; set; }

    public required string CandidateId { get; set; }

    public required string? Note { get; set; }

    public required DateTimeOffset ReceivedAt { get; set; }
}
