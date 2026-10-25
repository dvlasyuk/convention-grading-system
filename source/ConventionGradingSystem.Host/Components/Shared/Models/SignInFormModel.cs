using MudBlazor;

namespace ConventionGradingSystem.Host.Components.Shared.Models;

public record class SignInFormModel(
    SignInFormModel.FormModel Form,
    SignInFormModel.PasswordInputModel Input,
    bool IsFormValid)
{
    public bool IsFormValid { get; set; } = IsFormValid;

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
