namespace ConventionGradingSystem.Models.ParticipantVoteForm;

/// <summary>
/// Модель представления формы приложения для сбора голосов за участников зрительского голосования.
/// </summary>
/// <param name="VotingName">Человеко-читаемое название голосования.</param>
/// <param name="FriendlyVoting">Признак, допустимо ли зрителям отдавать голоса за участников, имеющих
/// прямое отношение к тому же отряду, что и сами зрители.</param>
/// <param name="VotesQuantity">Количество голосов, которые может отдать каждый зритель.</param>
/// <param name="Participants">Информация об участниках голосования.</param>
public record ViewModel(
    string VotingName,
    bool FriendlyVoting,
    int VotesQuantity,
    IReadOnlyList<VotingParticipant> Participants);
