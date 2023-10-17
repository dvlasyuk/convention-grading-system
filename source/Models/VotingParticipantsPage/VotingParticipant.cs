namespace ConventionGradingSystem.Models.VotingParticipantsPage;

/// <summary>
/// Информация об участнике зрительского голосования.
/// </summary>
/// <param name="Identifier">Идентификатор участника.</param>
/// <param name="Name">Название/имя участника.</param>
/// <param name="VotesQuantity">Количесто голосов, отданных за участника.</param>
/// <param name="Brigades">Названия связанных с участником отрядов.</param>
public record VotingParticipant(
    string Identifier,
    string Name,
    int VotesQuantity,
    IReadOnlyCollection<string> Brigades);
