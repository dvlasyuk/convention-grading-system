using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Configuration.Validators;

public class SecurityConfigurationValidator : IValidateOptions<SecurityConfiguration>
{
    public ValidateOptionsResult Validate(string? name, [NotNull] SecurityConfiguration options)
    {
        var failureMessages = new List<string>();

        if (string.IsNullOrEmpty(options.AdministratorSecret))
        {
            failureMessages.Add("Для секретной фразы администратора задано пустое значение");
        }
        else if (options.AdministratorSecret.Length > 100)
        {
            failureMessages.Add("Для секретной фразы администратора задано значение, превышающее 100 символов");
        }

        if (string.IsNullOrEmpty(options.OrganizerSecret))
        {
            failureMessages.Add("Для секретной фразы организатора задано пустое значение");
        }
        else if (options.OrganizerSecret.Length > 100)
        {
            failureMessages.Add("Для секретной фразы организатора задано значение, превышающее 100 символов");
        }

        return failureMessages.Any()
            ? ValidateOptionsResult.Fail(failureMessages)
            : ValidateOptionsResult.Success;
    }
}
