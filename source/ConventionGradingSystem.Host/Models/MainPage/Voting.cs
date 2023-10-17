namespace ConventionGradingSystem.Host.Models.MainPage;

/// <summary>
/// Информация о зрительском голосовании.
/// </summary>
/// <param name="Identifier">Идентификатор голосования.</param>
/// <param name="Name">Человеко-читаемое название голосования.</param>
/// <param name="ParticipantsQuantity">Количество участников голосования.</param>
public record Voting(
    string Identifier,
    string Name,
    int ParticipantsQuantity);
