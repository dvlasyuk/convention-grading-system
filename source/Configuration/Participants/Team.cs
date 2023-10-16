namespace ConventionGradingSystem.Configuration.Participants;

public class Team
{
    public string Identifier { get; set; } = string.Empty;
    public string Name { get; set; } = "Неизвестная команда";
    public ICollection<Participant> Members { get; } = new List<Participant>();
}
