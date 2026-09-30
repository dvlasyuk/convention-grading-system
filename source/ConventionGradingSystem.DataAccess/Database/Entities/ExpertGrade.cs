namespace ConventionGradingSystem.DataAccess.Database.Entities;

public class ExpertGrade
{
    public Guid FeedbackId { get; set; }

    public required string CriterionId { get; set; }

    public required int GradeValue { get; set; }

    public ExpertFeedback? Feedback { get; set; }
}
