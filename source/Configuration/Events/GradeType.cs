namespace ConventionGradingSystem.Configuration.Events;

public class GradeType
{
    public int Identifier { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int MinimalGrade { get; set; }
    public int MaximalGrade { get; set; }
}
