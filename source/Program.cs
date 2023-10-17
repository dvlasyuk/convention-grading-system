using ConventionGradingSystem.Configuration;
using ConventionGradingSystem.Configuration.Validators;
using ConventionGradingSystem.Database;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem;

/// <summary>
/// Основной класс приложения, содержащий точку входа.
/// </summary>
public static class Program
{
    /// <summary>
    /// Основной метод приложения, являющийся точкой входа.
    /// </summary>
    /// <param name="arguments">Аргументы командной строки, переданные приложению.</param>
    public static void Main(string[] arguments)
    {
        var applicationBuilder = WebApplication.CreateBuilder(arguments);

        applicationBuilder.Services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(cookieOptions =>
            {
                cookieOptions.LoginPath = "/login";
                cookieOptions.AccessDeniedPath = "/denied";
            });

        applicationBuilder.Services.AddAuthorization();

        applicationBuilder.Services.AddRazorPages(options => options.Conventions
            .ConfigureFilter(new IgnoreAntiforgeryTokenAttribute()));

        applicationBuilder.Services.AddDbContextPool<DatabaseContext>(builder => builder
            .UseSqlite(applicationBuilder.Configuration.GetConnectionString("Database"))
            .EnableSensitiveDataLogging());

        applicationBuilder.Services.AddSingleton<IValidateOptions<ApplicationConfiguration>, ApplicationConfigurationValidator>();
        applicationBuilder.Services.AddSingleton<IValidateOptions<SecurityConfiguration>, SecurityConfigurationValidator>();

        applicationBuilder.Services.Configure<ApplicationConfiguration>(applicationBuilder.Configuration.GetSection("ApplicationConfiguration"));
        applicationBuilder.Services.Configure<SecurityConfiguration>(applicationBuilder.Configuration.GetSection("SecurityConfiguration"));

        var application = applicationBuilder.Build();

        using var migrationScope = application.Services.CreateScope();
        var databaseContext = migrationScope.ServiceProvider.GetRequiredService<DatabaseContext>();
        databaseContext.Database.Migrate();

        application.UseRouting();
        application.UseAuthentication();
        application.UseAuthorization();
        application.MapRazorPages();

        application.Run();
    }
}
