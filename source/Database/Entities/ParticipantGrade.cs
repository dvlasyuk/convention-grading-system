namespace ConventionGradingSystem.Database.Entities;

public class ParticipantGrade
{
    public Guid Identifier { get; set; }
    public Guid FeedbackId { get; set; }
    public required string CriterionId { get; set; }
    public required int GradeValue { get; set; }
    public ParticipantFeedback? Feedback { get; set; }
}
