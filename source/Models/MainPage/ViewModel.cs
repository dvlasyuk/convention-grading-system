namespace ConventionGradingSystem.Models.MainPage;

/// <summary>
/// Модель преставления главной страницы приложения.
/// </summary>
/// <param name="Contests">Информация о конкурсах мероприятий.</param>
/// <param name="Teams">Информация о командах участников.</param>
public record ViewModel(
    IReadOnlyList<Contest> Contests,
    IReadOnlyList<Team> Teams);
