namespace ConventionGradingSystem.Configuration;

/// <summary>
/// Конфигурационные данные для обеспечения безопасности приложения.
/// </summary>
public class SecurityConfiguration
{
    /// <summary>
    /// Хэш секретной фразы администратора.
    /// </summary>
    /// <remarks>Значение не должно быть пустым и его длина не должна превышать 100 символов.</remarks>
    public string AdministratorSecretHash { get; set; } = string.Empty;

    /// <summary>
    /// Хэш секретной фразы организатора.
    /// </summary>
    /// <remarks>Значение не должно быть пустым и его длина не должна превышать 100 символов.</remarks>
    public string OrganizerSecretHash { get; set; } = string.Empty;

    /// <summary>
    /// Хэш секретной фразы эксперта.
    /// </summary>
    /// <remarks>Значение не должно быть пустым и его длина не должна превышать 100 символов.</remarks>
    public string ExpertSecretHash { get; set; } = string.Empty;
}
