namespace ConventionGradingSystem.Host.Models.MainPage;

/// <summary>
/// Модель преставления главной страницы приложения.
/// </summary>
/// <param name="Contests">Информация о конкурсах мероприятий.</param>
/// <param name="Votings">Информация о зрительских голосованиях.</param>
/// <param name="Teams">Информация о командах участников.</param>
public record ViewModel(
    IReadOnlyList<Contest> Contests,
    IReadOnlyList<Voting> Votings,
    IReadOnlyList<Team> Teams);
