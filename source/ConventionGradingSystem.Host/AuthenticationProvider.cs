using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using ConventionGradingSystem.Host.Configuration;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host;

/// <summary>
/// Провайдер информации об аутентифицированном пользователе.
/// </summary>
public class AuthenticationProvider : AuthenticationStateProvider
{
    private readonly SecurityConfiguration _configuration;
    private readonly ProtectedLocalStorage _localStorage;

    /// <summary>
    /// Создаёт новый экземпляр <see cref="AuthenticationProvider"/>.
    /// </summary>
    /// <param name="configuration">Конфигурационные данные для обеспечения безопасности приложения.</param>
    /// <param name="localStorage">Локальное хранилище браузера.</param>
    public AuthenticationProvider(
        [NotNull] IOptions<SecurityConfiguration> configuration,
        [NotNull] ProtectedLocalStorage localStorage)
    {
        _configuration = configuration.Value;
        _localStorage = localStorage;
    }

    /// <summary>
    /// Возвращает состояние аутентифицированного пользователя.
    /// </summary>
    /// <returns>Состояние аутентифицированного пользователя.</returns>
    [SuppressMessage("Design", "CA1031: Do not catch general exception types")]
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var principal = new ClaimsPrincipal();

        try
        {
            var result = await _localStorage.GetAsync<string>("Identity");
            if (result.Success && result.Value != null)
            {
                principal = CreatePrincipal(result.Value);
            }
        }
        catch
        { }

        return new AuthenticationState(principal);
    }

    /// <summary>
    /// Осуществляет вход пользователя по секретной фразе.
    /// </summary>
    /// <param name="secretPhrase">Секретная фраза пользователя.</param>
    public async Task SignIn(string secretPhrase)
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
            role = "Administrator";
        }
        else if (string.Equals(hashedSecret, _configuration.OrganizerSecretHash, StringComparison.Ordinal))
        {
            role = "Organizer";
        }
        else if (string.Equals(hashedSecret, _configuration.ExpertSecretHash, StringComparison.Ordinal))
        {
            role = "Expert";
        }

        var principal = new ClaimsPrincipal();
        if (role != null)
        {
            principal = CreatePrincipal(role);
            await _localStorage.SetAsync("Identity", role);
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    /// <summary>
    /// Осуществляет выход пользователя.
    /// </summary>
    public async Task SignOut()
    {
        await _localStorage.DeleteAsync("Identity");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
    }

    private static ClaimsPrincipal CreatePrincipal(string role)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(
            claims: new List<Claim>
            {
                new(ClaimTypes.Name, role),
                new(ClaimTypes.Role, role)
            },
            authenticationType: "ConventonGradingSystem"));
    }
}
