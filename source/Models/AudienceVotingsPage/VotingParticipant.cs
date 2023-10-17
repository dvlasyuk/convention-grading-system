namespace ConventionGradingSystem.Models.AudienceVotingsPage;

/// <summary>
/// Информация об участнике зрительского голосования.
/// </summary>
/// <param name="Name">Название/имя участника.</param>
/// <param name="Brigades">Названия связанных с участником отрядов.</param>
public record VotingParticipant(
    string Name,
    IReadOnlyCollection<string> Brigades);
