namespace ConventionGradingSystem.Configuration.Contests;

public class Contest
{
    public int Identifier { get; set; } = int.MinValue;
    public string Name { get; set; } = "Неизвестный конкурс";
    public ICollection<GradeCriterion> ExpertCriterions { get; } = new List<GradeCriterion>();
    public ICollection<GradeCriterion> ParticipantCriterions { get; } = new List<GradeCriterion>();
    public ICollection<ContestEvent> Events { get; } = new List<ContestEvent>();
}
