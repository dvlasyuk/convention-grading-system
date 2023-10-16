namespace ConventionGradingSystem.Configuration.Models;

public class Participant
{
    public string Identifier { get; set; } = string.Empty;
    public string Name { get; set; } = "Неизвестное имя";
    public string Brigade { get; set; } = "Неизвестный отряд";
}