namespace ConventionGradingSystem.Models.ParticipantVoteForm;

/// <summary>
/// Информация об участнике зрительского голосования.
/// </summary>
/// <param name="Identifier">Идентификатор участника.</param>
/// <param name="Name">Человеко-читаемое название/имя участника.</param>
public record VotingParticipant(
    string Identifier,
    string Name);
