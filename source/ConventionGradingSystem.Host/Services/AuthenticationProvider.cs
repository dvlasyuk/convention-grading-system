using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using ConventionGradingSystem.Host.Configuration;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Services;

public class AuthenticationProvider : AuthenticationStateProvider
{
    private const string _authenticationType = "ConventonGradingSystem";
    private const string _localStorageKey = "Identity";

    private readonly SecurityConfiguration _configuration;
    private readonly ProtectedLocalStorage _localStorage;

    public AuthenticationProvider(
        [NotNull] IOptions<SecurityConfiguration> configuration,
        [NotNull] ProtectedLocalStorage localStorage)
    {
        _configuration = configuration.Value;
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var principal = new ClaimsPrincipal();

        var result = await _localStorage.GetAsync<string>(_localStorageKey);
        if (result is { Success: true, Value: not null })
        {
            principal = CreatePrincipal(result.Value);
        }

        return new AuthenticationState(principal);
    }

    public async Task<bool> SignIn(string secretPhrase)
    {
        ArgumentNullException.ThrowIfNull(secretPhrase);

        var secretBytes = Encoding.UTF8.GetBytes(secretPhrase);
        var hashedBytes = SHA256.HashData(secretBytes);
        var hashedSecret = string.Empty;
        foreach (var item in hashedBytes)
        {
            hashedSecret += $"{item:x2}";
        }

        string? role = null;
        if (string.Equals(hashedSecret, _configuration.AdministratorSecretHash, StringComparison.Ordinal))
        {
            role = AuthenticationConstants.AdministratorRole;
        }
        else if (string.Equals(hashedSecret, _configuration.FacilitatorSecretHash, StringComparison.Ordinal))
        {
            role = AuthenticationConstants.FacilitatorRole;
        }
        else if (string.Equals(hashedSecret, _configuration.ExpertSecretHash, StringComparison.Ordinal))
        {
            role = AuthenticationConstants.ExpertRole;
        }

        var principal = new ClaimsPrincipal();
        if (role is not null)
        {
            principal = CreatePrincipal(role);
            await _localStorage.SetAsync(_localStorageKey, role);
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));

        return role is not null;
    }

    public async Task SignOut()
    {
        await _localStorage.DeleteAsync(_localStorageKey);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
    }

    private static ClaimsPrincipal CreatePrincipal(string role)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(
            claims: [
                new(ClaimTypes.Name, role),
                new(ClaimTypes.Role, role)],
            authenticationType: _authenticationType));
    }
}
