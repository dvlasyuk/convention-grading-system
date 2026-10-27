using ConventionGradingSystem.Host.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace ConventionGradingSystem.Host.Components.Shared.Components;

public partial class UnathorizedView
{
    private readonly NavigationManager _navigationManager;

    [CascadingParameter]
    private Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;
    private bool IsUserAuthenticated { get; set; }

    public UnathorizedView(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    protected override async Task OnInitializedAsync()
    {
        var authenticationState = await AuthenticationStateTask;
        if (authenticationState.User.Identity is { IsAuthenticated: true })
        {
            IsUserAuthenticated = true;
            return;
        }

        var returnUri = _navigationManager.ToBaseRelativePath(_navigationManager.Uri);
        _navigationManager.NavigateTo(NavigationHelper.SignInPage(returnUri));
    }
}
