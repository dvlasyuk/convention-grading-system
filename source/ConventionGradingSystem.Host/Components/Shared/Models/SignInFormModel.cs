using MudBlazor;

namespace ConventionGradingSystem.Host.Components.Shared.Models;

public record class SignInFormModel
{
    public string? EnteredSecret { get; set; }
    public bool IsFormValid { get; set; }

    public bool IsPasswordShowed { get; set; }
    public InputType PasswordInputType { get; set; } = InputType.Password;
    public string PasswordInputIcon { get; set; } = Icons.Material.Filled.VisibilityOff;
}
