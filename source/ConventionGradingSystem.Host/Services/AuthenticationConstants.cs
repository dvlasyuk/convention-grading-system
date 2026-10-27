namespace ConventionGradingSystem.Host.Services;

public static class AuthenticationConstants
{
    public const string AdministratorRole = "Administrator";
    public const string OrganizerRole = "Organizer";
    public const string ExpertRole = "Expert";

    public const string AdministratorAndOrganizerRoles = $"{AdministratorRole},{OrganizerRole}";
    public const string AdministratorAndExpertRoles = $"{AdministratorRole},{ExpertRole}";
}
