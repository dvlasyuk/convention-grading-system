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

public class LoginFormModel : PageModel
{
    private readonly SecurityConfiguration _configuration;

    public LoginFormModel([NotNull] IOptionsSnapshot<SecurityConfiguration> configuration) =>
        _configuration = configuration.Value;

    [BindProperty]
    public FormModel? FormModel { get; set; }

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
