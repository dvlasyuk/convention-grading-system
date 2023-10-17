namespace ConventionGradingSystem.Host.Models.VotingParticipantsPage;

/// <summary>
/// Модель представления страницы приложения со списком участников зрительского голосования.
/// </summary>
/// <param name="VotingName">Название голосования.</param>
/// <param name="Participants">Информация об участниках голосования.</param>
public record ViewModel(
    string VotingName,
    IReadOnlyList<VotingParticipant> Participants);
