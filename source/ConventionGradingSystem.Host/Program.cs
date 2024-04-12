using ConventionGradingSystem.DataAccess;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.Host.Configuration;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using MudBlazor.Services;

namespace ConventionGradingSystem.Host;

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

        applicationBuilder.Services.AddRazorPages();
        applicationBuilder.Services.AddServerSideBlazor();
        applicationBuilder.Services.AddMudServices();

        applicationBuilder.Services.AddOptions<SecurityConfiguration>().BindConfiguration("SecurityConfiguration");
        applicationBuilder.Services.AddSingleton<IValidateOptions<SecurityConfiguration>, SecurityConfigurationValidator>();

        applicationBuilder.Services.AddScoped<AuthenticationProvider>();
        applicationBuilder.Services.AddScoped<AuthenticationStateProvider, AuthenticationProvider>();

        applicationBuilder.Services.AddDataAccess(
            configurationSection: "ApplicationConfiguration",
            connectionString: "ApplicationDatabase");

        var application = applicationBuilder.Build();

        using var migrationScope = application.Services.CreateScope();
        var databaseContext = migrationScope.ServiceProvider.GetRequiredService<DatabaseContext>();
        databaseContext.Database.Migrate();

        application.UseStaticFiles();
        application.UseRouting();
        application.MapBlazorHub();
        application.MapFallbackToPage("/Application");

        application.Run();
    }
}
