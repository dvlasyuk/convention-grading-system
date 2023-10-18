using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.DataAccess;

/// <summary>
/// Методы расширения, предназначенные для регистрации функциональности доступа к данным.
/// </summary>
public static class RegistrationExtensions
{
    /// <summary>
    /// Регистрирует сервисы, необходимые для доступа к данным в заданном экземпляре <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">Коллекция, в которой требуется зарегистрировать необходмиые сервисы.</param>
    /// <param name="configurationSection">Название секции конфигурации приложения, из которой должны быть
    /// получены конфигурационные данные приложения.</param>
    /// <param name="connectionString">Название строки подключения к базе данных, в которой должны храниться
    /// операционные данные приложения.</param>
    /// <returns>Тот же экземпляр <see cref="IServiceCollection"/>, что позволяет создать цепочку вызовов.</returns>
    public static IServiceCollection AddDataAccess(
        this IServiceCollection services,
        string configurationSection,
        string connectionString)
    {
        services.AddOptions<ApplicationConfiguration>().BindConfiguration(configurationSection);
        services.AddSingleton<IValidateOptions<ApplicationConfiguration>, ApplicationConfigurationValidator>();

        services.AddDbContextPool<DatabaseContext>((provider, builder) =>
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
