namespace ConventionGradingSystem.Configuration.Models;

public class Contest
{
    public string Identifier { get; set; } = string.Empty;
    public string Name { get; set; } = "Неизвестный конкурс";
    public ICollection<GradeCriterion> ExpertCriterions { get; } = new List<GradeCriterion>();
    public ICollection<GradeCriterion> ParticipantCriterions { get; } = new List<GradeCriterion>();
    public ICollection<ContestEvent> Events { get; } = new List<ContestEvent>();
}
