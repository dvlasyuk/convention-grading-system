using ConventionGradingSystem.Blazor.Configuration;
using ConventionGradingSystem.DataAccess;
using ConventionGradingSystem.DataAccess.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Blazor;

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

        applicationBuilder.Services.AddOptions<SecurityConfiguration>().BindConfiguration("SecurityConfiguration");
        applicationBuilder.Services.AddSingleton<IValidateOptions<SecurityConfiguration>, SecurityConfigurationValidator>();

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
