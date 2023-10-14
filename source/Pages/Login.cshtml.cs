using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading.Tasks;

using ConventionGradingSystem.Configuration;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SecurityConfiguration _configuration;

        public LoginModel([NotNull] IOptionsSnapshot<SecurityConfiguration> configuration)
        {
            _configuration = configuration.Value;
        }

        [BindProperty]
        public string Secret { get; set; }

        public async Task<IActionResult> OnPost(string returnUrl)
        {
            string user;
            if (string.Equals(Secret, _configuration.AdministratorSecret))
            {
                user = "Adminstrator";
            }
            else if (string.Equals(Secret, _configuration.OrganizerSecret))
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

            return Redirect(string.IsNullOrEmpty(returnUrl) 
                ? "/Index"
                : returnUrl);
        }
    }
}
