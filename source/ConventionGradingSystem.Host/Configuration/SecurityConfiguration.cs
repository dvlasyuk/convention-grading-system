namespace ConventionGradingSystem.Host.Configuration;

public class SecurityConfiguration
{
    public string AdministratorSecretHash { get; set; } = string.Empty;

    public string FacilitatorSecretHash { get; set; } = string.Empty;

    public string ExpertSecretHash { get; set; } = string.Empty;
}
