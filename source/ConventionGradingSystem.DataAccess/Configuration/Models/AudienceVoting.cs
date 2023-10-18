namespace ConventionGradingSystem.DataAccess.Configuration.Models;

/// <summary>
/// Конфигурация зрительского голосования.
/// </summary>
public class AudienceVoting
{
    /// <summary>
    /// Идентификатор голосования.
    /// </summary>
    /// <remarks>Значение не должно быть пустым и его длина не должна превышать 50 символов. Значение
    /// должно быть уникальным для всех сконфигурированных голосований.</remarks>
    public string Identifier { get; set; } = string.Empty;

    /// <summary>
    /// Человеко-читаемое название голосования.
    /// </summary>
    /// <remarks>Значение не должно быть пустым и его длина не должна превышать 100 символов.</remarks>
    public string Name { get; set; } = "Неизвестное голосование";

    /// <summary>
    /// Признак, допустимо ли зрителям отдавать голоса за участников, имеющих прямое отношение к тому
    /// же отряду, что и сами зрители.
    /// </summary>
    public bool FriendlyVoting { get; set; } = true;

    /// <summary>
    /// Количество голосов, которые может отдать каждый зритель.
    /// </summary>
    /// <remarks>Значение должно быть положительным и строго меньше количества сконфигурированных
    /// участников голосования.</remarks>
    public int VotesQuantity { get; set; } = int.MinValue;

    /// <summary>
    /// Участники зрительского голосования.
    /// </summary>
    /// <remarks>Количество сконфигурированных участников не может быть нулевым и не должно превышать
    /// 100 участников.</remarks>
    public ICollection<VotingParticipant> Participants { get; } = new List<VotingParticipant>();
}
