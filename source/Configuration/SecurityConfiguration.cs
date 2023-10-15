namespace ConventionGradingSystem.Configuration;

public class SecurityConfiguration
{
    public string AdministratorSecret { get; set; } = string.Empty;
    public string OrganizerSecret { get; set; } = string.Empty;
}
