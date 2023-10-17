namespace ConventionGradingSystem.Host.Models.LoginForm;

/// <summary>
/// Модель представления формы приложения для входа пользователя.
/// </summary>
/// <param name="Secret">Секретная фраза, введённая пользователем.</param>
public record FormModel(string Secret);
