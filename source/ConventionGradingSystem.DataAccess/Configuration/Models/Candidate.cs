namespace ConventionGradingSystem.DataAccess.Configuration.Models;

public class Candidate
{
    public string Identifier { get; set; } = string.Empty;

    public string Name { get; set; } = "Неизвестный участник";

    public ICollection<string> Brigades { get; } = [];

    public ICollection<string> Teams { get; } = [];
}
