namespace ConventionGradingSystem.Models.ParticipantVoteForm;

/// <summary>
/// Состояние формы приложения для сбора голосов за участников зрительского голосования.
/// </summary>
public enum FormState
{
    /// <summary>
    /// Заданное голосование не существует.
    /// </summary>
    NotExisted,

    /// <summary>
    /// Голос в заданном голосовании ещё не оставлен.
    /// </summary>
    NotVoted,

    /// <summary>
    /// Голос в заданном голосовании оставлен только что.
    /// </summary>
    JustVoted,

    /// <summary>
    /// Голос в заданном голосовании был оставлен ранее.
    /// </summary>
    PreviouslyVoted
}
