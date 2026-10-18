using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace ConventionGradingSystem.Host.Components.Shared.Components;

public partial class SignInForm
{
    private readonly AuthenticationProvider _authenticationProvider;
    private readonly NavigationManager _navigationManager;

    private MudForm? _formSelector;
    private string? _enteredSecret;
    private bool _isFormValid;

    private bool _isPasswordShowed;
    private InputType _passwordInputType = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    public SignInForm(
        AuthenticationProvider authenticationProvider,
        NavigationManager navigationManager)
    {
        _authenticationProvider = authenticationProvider;
        _navigationManager = navigationManager;
    }

    private async Task SignInToSystem()
    {
        await _formSelector!.ValidateAsync();
        if (!_isFormValid)
        {
            return;
        }

        await _authenticationProvider.SignIn(_enteredSecret!);
        _navigationManager.NavigateTo(_navigationManager.Uri, true);
    }

    private void ShowPassword()
    {
        if (_isPasswordShowed)
        {
            _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            _passwordInputType = InputType.Password;
        }
        else
        {
            _passwordInputIcon = Icons.Material.Filled.Visibility;
            _passwordInputType = InputType.Text;
        }

        _isPasswordShowed = !_isPasswordShowed;
    }
}
