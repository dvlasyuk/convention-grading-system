using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Configuration.Validators;

public class SecurityConfigurationValidator : IValidateOptions<SecurityConfiguration>
{
    public ValidateOptionsResult Validate(string? name, [NotNull] SecurityConfiguration options)
    {
        var failureMessages = new List<string>();

        if (string.IsNullOrEmpty(options.AdministratorSecretHash))
        {
            failureMessages.Add("Для секретной фразы администратора задан пустой хэш");
        }
        else if (options.AdministratorSecretHash.Length > 100)
        {
            failureMessages.Add("Для секретной фразы администратора задан хэш, превышающий 100 символов");
        }

        if (string.IsNullOrEmpty(options.OrganizerSecretHash))
        {
            failureMessages.Add("Для секретной фразы организатора задан пустой хэш");
        }
        else if (options.OrganizerSecretHash.Length > 100)
        {
            failureMessages.Add("Для секретной фразы организатора задан хэш, превышающий 100 символов");
        }

        if (string.IsNullOrEmpty(options.ExpertSecretHash))
        {
            failureMessages.Add("Для секретной фразы эксперта задан пустой хэш");
        }
        else if (options.ExpertSecretHash.Length > 100)
        {
            failureMessages.Add("Для секретной фразы эксперта задан хэш, превышающий 100 символов");
        }

        return failureMessages.Any()
            ? ValidateOptionsResult.Fail(failureMessages)
            : ValidateOptionsResult.Success;
    }
}
