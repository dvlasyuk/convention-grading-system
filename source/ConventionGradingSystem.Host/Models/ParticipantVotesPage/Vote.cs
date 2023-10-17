namespace ConventionGradingSystem.Host.Models.ParticipantVotesPage;

/// <summary>
/// Информация о голсове, оставленном за участника зрительского голосования.
/// </summary>
/// <param name="ParticipantName">Имя участника, оставившего голос.</param>
/// <param name="ParticipantBrigade">Название отряда участника, оставившегося голос.</param>
/// <param name="Note">Дополнительный комментарий, оставленный участником.</param>
public record Vote(
    string ParticipantName,
    string ParticipantBrigade,
    string? Note);
