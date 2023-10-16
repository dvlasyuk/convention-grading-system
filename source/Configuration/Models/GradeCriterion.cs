namespace ConventionGradingSystem.Configuration.Models;

public class GradeCriterion
{
    public string Identifier { get; set; } = string.Empty;
    public string Name { get; set; } = "Неизвестный критерий";
    public string Description { get; set; } = "Неизвестный критерий";
    public int MinimalGrade { get; set; } = int.MinValue;
    public int MaximalGrade { get; set; } = int.MinValue;
}
