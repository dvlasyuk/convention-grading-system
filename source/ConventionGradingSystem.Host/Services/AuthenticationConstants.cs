namespace ConventionGradingSystem.Host.Services;

public static class AuthenticationConstants
{
    public const string AdministratorRole = "Administrator";
    public const string FacilitatorRole = "Facilitator";
    public const string ExpertRole = "Expert";

    public const string AdministratorAndFacilitatorRoles = $"{AdministratorRole},{FacilitatorRole}";
    public const string AdministratorAndExpertRoles = $"{AdministratorRole},{ExpertRole}";
}
