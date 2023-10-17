using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using ConventionGradingSystem.Configuration;
using ConventionGradingSystem.Models.LoginForm;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Pages;

/// <summary>
/// Модель формы приложения для входа пользователя.
/// </summary>
public class LoginFormModel : PageModel
{
    private readonly SecurityConfiguration _configuration;

    /// <summary>
    /// Создаёт новый экземпляр <see cref="LoginFormModel"/>.
    /// </summary>
    /// <param name="configuration">Конфигурационные данные для обеспечения безопасности приложения.</param>
    public LoginFormModel([NotNull] IOptionsSnapshot<SecurityConfiguration> configuration) =>
        _configuration = configuration.Value;

    /// <summary>
    /// Модель представления страницы.
    /// </summary>
    [BindProperty]
    public FormModel? FormModel { get; set; }

    /// <summary>
    /// Обрабатывает POST-запрос к странице.
    /// </summary>
    /// <param name="returnUrl">URI для автоматического перехода при успешном входе пользователя.</param>
    public async Task<IActionResult> OnPost(Uri returnUrl)
    {
        if (FormModel == null)
        {
            throw new InvalidOperationException("Модель формы должна быть заполнена при выполнении POST-запроса");
        }

        var secretBytes = Encoding.UTF8.GetBytes(FormModel.Secret);
        var hashedBytes = SHA256.HashData(secretBytes);
        var hashedSecret = string.Empty;
        foreach (var item in hashedBytes)
        {
            hashedSecret += $"{item:x2}";
        }

        string user;
        if (string.Equals(hashedSecret, _configuration.AdministratorSecretHash, StringComparison.Ordinal))
        {
            user = "Adminstrator";
        }
        else if (string.Equals(hashedSecret, _configuration.OrganizerSecretHash, StringComparison.Ordinal))
        {
            user = "Organizer";
        }
        else if (string.Equals(hashedSecret, _configuration.ExpertSecretHash, StringComparison.Ordinal))
        {
            user = "Expert";
        }
        else
        {
            return Page();
        }

        await HttpContext.SignInAsync(
            scheme: CookieAuthenticationDefaults.AuthenticationScheme,
            principal: new ClaimsPrincipal(new ClaimsIdentity(
                claims: new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user),
                    new Claim(ClaimTypes.Role, user)
                },
                authenticationType: CookieAuthenticationDefaults.AuthenticationScheme)));

        return Redirect(returnUrl?.ToString() ?? "/Index");
    }
}
