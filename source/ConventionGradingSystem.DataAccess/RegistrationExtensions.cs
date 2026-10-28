using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.DataAccess;

public static class RegistrationExtensions
{
    public static IServiceCollection AddDataAccess(
        this IServiceCollection services,
        string configurationSection,
        string connectionString)
    {
        services.AddOptions<DataConfiguration>().BindConfiguration(configurationSection);
        services.AddSingleton<IValidateOptions<DataConfiguration>, DataConfigurationValidator>();

        services.AddDbContextFactory<DatabaseContext>((provider, builder) =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            builder
                .UseSqlite(configuration.GetConnectionString(connectionString))
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
        });

        return services;
    }
}
