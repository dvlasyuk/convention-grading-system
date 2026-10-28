using System.Globalization;

using ConventionGradingSystem.DataAccess;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.Host.Configuration;
using ConventionGradingSystem.Host.Services;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using MudBlazor.Services;

namespace ConventionGradingSystem.Host;

public static class Program
{
    private const string _applicationCulture = "ru-RU";

    public static void Main(string[] arguments)
    {
        var applicationBuilder = WebApplication.CreateBuilder(arguments);

        applicationBuilder.Services.AddRazorPages();
        applicationBuilder.Services.AddServerSideBlazor();
        applicationBuilder.Services.AddMudServices();

        applicationBuilder.Services.AddOptions<ApplicationConfiguration>().BindConfiguration("ApplicationConfiguration");
        applicationBuilder.Services.AddOptions<SecurityConfiguration>().BindConfiguration("SecurityConfiguration");
        applicationBuilder.Services.AddSingleton<IValidateOptions<ApplicationConfiguration>, ApplicationConfigurationValidator>();
        applicationBuilder.Services.AddSingleton<IValidateOptions<SecurityConfiguration>, SecurityConfigurationValidator>();

        applicationBuilder.Services.AddCascadingAuthenticationState();
        applicationBuilder.Services.AddScoped<AuthenticationProvider>();
        applicationBuilder.Services.AddScoped<AuthenticationStateProvider, AuthenticationProvider>();

        applicationBuilder.Services.AddDataAccess(
            configurationSection: "DataConfiguration",
            connectionString: "ApplicationDatabase");

        var application = applicationBuilder.Build();

        using var migrationScope = application.Services.CreateScope();
        var databaseContext = migrationScope.ServiceProvider.GetRequiredService<DatabaseContext>();
        databaseContext.Database.Migrate();

        application.UseRequestLocalization(options =>
        {
            options.DefaultRequestCulture = new RequestCulture(_applicationCulture);
            options.SupportedCultures = [new CultureInfo(_applicationCulture)];
            options.SupportedUICultures = [new CultureInfo(_applicationCulture)];
        });

        application.UseStaticFiles();
        application.UseRouting();
        application.MapBlazorHub();
        application.MapFallbackToPage("/Application");

        application.Run();
    }
}
