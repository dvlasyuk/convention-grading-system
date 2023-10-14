using ConventionGradingSystem.Configuration;
using ConventionGradingSystem.Database;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConventionGradingSystem
{
    public static class Program
    {
        public static void Main(string[] arguments)
        {
            var applicationBuilder = WebApplication.CreateBuilder(arguments);

            applicationBuilder.Services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(cookieOptions =>
                {
                    cookieOptions.LoginPath = "/Login";
                    cookieOptions.AccessDeniedPath = "/AccessDenied";
                });

            applicationBuilder.Services.AddAuthorization();

            applicationBuilder.Services.AddRazorPages(options => options.Conventions
                .ConfigureFilter(new IgnoreAntiforgeryTokenAttribute()));

            applicationBuilder.Services.AddDbContextPool<DatabaseContext>(builder => builder
                .UseSqlite(applicationBuilder.Configuration.GetConnectionString("Database"))
                .EnableSensitiveDataLogging());

            applicationBuilder.Services
                .AddOptions<ApplicationConfiguration>()
                .BindConfiguration("ApplicationConfiguration");

            applicationBuilder.Services
                .AddOptions<SecurityConfiguration>()
                .BindConfiguration("SecurityConfiguration");

            var application = applicationBuilder.Build();

            using var migrationScope = application.Services.CreateScope();
            var databaseContext = migrationScope.ServiceProvider.GetRequiredService<DatabaseContext>();
            databaseContext.Database.Migrate();

            application.UseRouting();
            application.UseAuthentication();
            application.UseAuthorization();
            application.UseEndpoints(endpoints => endpoints.MapRazorPages());

            application.Run();
        }
    }
}
