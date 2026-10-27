using MudBlazor;

namespace ConventionGradingSystem.Host.Components.Shared.Models;

public record SignInPageModel(
    SignInPageModel.FormModel Form,
    SignInPageModel.PasswordInputModel Input,
    bool IsFormValid,
    bool IsSignInFailed)
{
    public bool IsFormValid { get; set; } = IsFormValid;
    public bool IsSignInFailed { get; set; } = IsSignInFailed;

    public record FormModel(string? EnteredSecret)
    {
        public string? EnteredSecret { get; set; } = EnteredSecret;
    }

    public record PasswordInputModel(
        bool IsPasswordShowed,
        InputType InputType,
        string InputIcon)
    {
        public bool IsPasswordShowed { get; set; } = IsPasswordShowed;
        public InputType InputType { get; set; } = InputType;
        public string InputIcon { get; set; } = InputIcon;
    }
}
