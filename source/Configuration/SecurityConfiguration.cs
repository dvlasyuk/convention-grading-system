namespace ConventionGradingSystem.Configuration;

public class SecurityConfiguration
{
    public string AdministratorSecretHash { get; set; } = string.Empty;
    public string OrganizerSecretHash { get; set; } = string.Empty;
    public string ExpertSecretHash { get; set; } = string.Empty;
}
