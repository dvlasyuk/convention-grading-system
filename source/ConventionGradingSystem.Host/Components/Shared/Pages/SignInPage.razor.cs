using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Host.Components.Shared.Models;
using ConventionGradingSystem.Host.Services;

using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace ConventionGradingSystem.Host.Components.Shared.Pages;

[SuppressMessage("Design", "CA1056:URI-like properties should not be strings")]
public partial class SignInPage
{
    private readonly AuthenticationProvider _authenticationProvider;
    private readonly NavigationManager _navigationManager;

    private MudForm? _formSelector;

    [Parameter]
    [SupplyParameterFromQuery(Name = "return_uri")]
    public string? ReturnUri { get; set; }

    public SignInPage(
        AuthenticationProvider authenticationProvider,
        NavigationManager navigationManager)
    {
        _authenticationProvider = authenticationProvider;
        _navigationManager = navigationManager;
    }

    protected override Task<ContentState<SignInPageModel>> LoadContent()
    {
        var viewModel = new SignInPageModel(
            Form: new SignInPageModel.FormModel(
                EnteredSecret: null),
            Input: new SignInPageModel.PasswordInputModel(
                IsPasswordShowed: false,
                InputType: InputType.Password,
                InputIcon: Icons.Material.Filled.VisibilityOff),
            IsFormValid: false,
            IsSignInFailed: false);

        return Task.FromResult<ContentState<SignInPageModel>>(viewModel);
    }

    private async Task SignInToSystem()
    {
        await _formSelector!.ValidateAsync();
        if (!ViewModel.IsFormValid)
        {
            return;
        }

        var isSuccessful = await _authenticationProvider.SignIn(ViewModel.Form.EnteredSecret!);
        ViewModel.IsSignInFailed = !isSuccessful;

        if (!isSuccessful)
        {
            await _formSelector!.ValidateAsync();
            return;
        }

        if (!TryGetSafeReturnUri(out var safeReturnUri))
        {
            _navigationManager.NavigateTo(NavigationHelper.MainPage(), true);
            return;
        }

        _navigationManager.NavigateTo(safeReturnUri, true);
    }

    private string? CheckError()
    {
        return ViewModel.IsSignInFailed
            ? "Ты ввёл неверную секретную фразу"
            : null;
    }

    private void ClearError()
    {
        ViewModel.IsSignInFailed = false;
    }

    private void ShowPassword()
    {
        if (ViewModel.Input.IsPasswordShowed)
        {
            ViewModel.Input.InputIcon = Icons.Material.Filled.VisibilityOff;
            ViewModel.Input.InputType = InputType.Password;
        }
        else
        {
            ViewModel.Input.InputIcon = Icons.Material.Filled.Visibility;
            ViewModel.Input.InputType = InputType.Text;
        }

        ViewModel.Input.IsPasswordShowed = !ViewModel.Input.IsPasswordShowed;
    }

    private bool TryGetSafeReturnUri([NotNullWhen(true)] out string? returnUri)
    {
        returnUri = null;

        if (!Uri.TryCreate(ReturnUri, UriKind.Relative, out var relativeUri))
        {
            return false;
        }

        var absoluteUri = _navigationManager.ToAbsoluteUri(relativeUri.ToString());
        var baseUri = new Uri(_navigationManager.BaseUri, UriKind.Absolute);
        if (!baseUri.IsBaseOf(absoluteUri))
        {
            return false;
        }

        var currentUri = new Uri(_navigationManager.Uri, UriKind.Absolute);
        if (currentUri == absoluteUri)
        {
            return false;
        }

        returnUri = relativeUri.ToString();
        return true;
    }
}
