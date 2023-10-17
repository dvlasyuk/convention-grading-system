namespace ConventionGradingSystem.Models.MainPage;

/// <summary>
/// Информация о конкурсе мероприятий.
/// </summary>
/// <param name="Identifier">Идентификатор конкурса.</param>
/// <param name="Name">Человеко-читаемое название конкурса.</param>
/// <param name="EventsQuantity">Количество мероприятий в рамках конкурса.</param>
public record Contest(
    string Identifier,
    string Name,
    int EventsQuantity);
