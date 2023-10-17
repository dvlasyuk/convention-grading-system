namespace ConventionGradingSystem.Host.Models.ParticipantVotesPage;

/// <summary>
/// Модель представления страницы приложения со списком голосов участников за участника зрительского голосования.
/// </summary>
/// <param name="VotingName">Человеко-читаемое название голосования.</param>
/// <param name="ParticipantName">Человеко-читаемое название/имя участника голосования.</param>
/// <param name="Votes">Информация о голосах, отданных за участника голосования.</param>
public record ViewModel(
    string VotingName,
    string ParticipantName,
    IReadOnlyList<Vote> Votes);
