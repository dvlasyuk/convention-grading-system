using ConventionGradingSystem.Host.Components.Shared.Models;

using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace ConventionGradingSystem.Host.Components.Shared.Components;

public partial class SignInForm
{
    private readonly AuthenticationProvider _authenticationProvider;
    private readonly NavigationManager _navigationManager;

    private readonly SignInFormModel _viewModel = new();
    private MudForm? _formSelector;

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
        if (!_viewModel.IsFormValid)
        {
            return;
        }

        await _authenticationProvider.SignIn(_viewModel.EnteredSecret!);
        _navigationManager.NavigateTo(_navigationManager.Uri, true);
    }

    private void ShowPassword()
    {
        if (_viewModel.IsPasswordShowed)
        {
            _viewModel.PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
            _viewModel.PasswordInputType = InputType.Password;
        }
        else
        {
            _viewModel.PasswordInputIcon = Icons.Material.Filled.Visibility;
            _viewModel.PasswordInputType = InputType.Text;
        }

        _viewModel.IsPasswordShowed = !_viewModel.IsPasswordShowed;
    }
}
