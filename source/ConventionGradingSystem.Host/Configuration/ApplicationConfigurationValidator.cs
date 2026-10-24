using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Configuration;

public class ApplicationConfigurationValidator : IValidateOptions<ApplicationConfiguration>
{
    public ValidateOptionsResult Validate(string? name, [NotNull] ApplicationConfiguration options)
    {
        var failureMessages = new List<string>();
        if (string.IsNullOrWhiteSpace(options.ConventionName))
        {
            failureMessages.Add($"Задано пустое название слёта, для которого работает система");
        }
        else if (options.ConventionName.Length > 100)
        {
            failureMessages.Add($"Задано название слёта, для которого работает система, превышающее 100 символов");
        }

        return failureMessages.Count > 0
            ? ValidateOptionsResult.Fail(failureMessages)
            : ValidateOptionsResult.Success;
    }
}
