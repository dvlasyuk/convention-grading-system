using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

using ConventionGradingSystem.Configuration;
using ConventionGradingSystem.Models.Login;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Pages;

public class LoginModel : PageModel
{
    private readonly SecurityConfiguration _configuration;

    public LoginModel([NotNull] IOptionsSnapshot<SecurityConfiguration> configuration) =>
        _configuration = configuration.Value;

    [BindProperty]
    public FormModel? FormModel { get; set; }

    public async Task<IActionResult> OnPost(Uri returnUrl)
    {
        if (FormModel == null)
        {
            throw new InvalidOperationException("Модель формы должна быть заполнена при выполнении POST-запроса");
        }

        string user;
        if (string.Equals(FormModel.Secret, _configuration.AdministratorSecret, StringComparison.Ordinal))
        {
            user = "Adminstrator";
        }
        else if (string.Equals(FormModel.Secret, _configuration.OrganizerSecret, StringComparison.Ordinal))
        {
            user = "Organizer";
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
