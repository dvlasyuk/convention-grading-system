namespace ConventionGradingSystem.Configuration.Events;

public class GradeType
{
    public int Identifier { get; set; } = int.MinValue;
    public string Name { get; set; } = "Неизвестный критерий";
    public string? Description { get; set; }
    public int MinimalGrade { get; set; } = int.MinValue;
    public int MaximalGrade { get; set; } = int.MinValue;
}
