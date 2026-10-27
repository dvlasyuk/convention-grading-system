using ConventionGradingSystem.Host.Components.Shared.Models;
using ConventionGradingSystem.Host.Services;

using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace ConventionGradingSystem.Host.Components.Shared.Components;

public partial class SignInForm
{
    private readonly AuthenticationProvider _authenticationProvider;
    private readonly NavigationManager _navigationManager;

    private readonly SignInFormModel _viewModel = new(
        Form: new SignInFormModel.FormModel(
            EnteredSecret: null),
        Input: new SignInFormModel.PasswordInputModel(
            IsPasswordShowed: false,
            InputType: InputType.Password,
            InputIcon: Icons.Material.Filled.VisibilityOff),
        IsFormValid: false);

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

        await _authenticationProvider.SignIn(_viewModel.Form.EnteredSecret!);
        _navigationManager.NavigateTo(_navigationManager.Uri, true);
    }

    private void ShowPassword()
    {
        if (_viewModel.Input.IsPasswordShowed)
        {
            _viewModel.Input.InputIcon = Icons.Material.Filled.VisibilityOff;
            _viewModel.Input.InputType = InputType.Password;
        }
        else
        {
            _viewModel.Input.InputIcon = Icons.Material.Filled.Visibility;
            _viewModel.Input.InputType = InputType.Text;
        }

        _viewModel.Input.IsPasswordShowed = !_viewModel.Input.IsPasswordShowed;
    }
}
