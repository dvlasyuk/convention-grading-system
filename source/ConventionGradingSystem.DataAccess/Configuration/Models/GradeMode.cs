namespace ConventionGradingSystem.DataAccess.Configuration.Models;

/// <summary>
/// Способ оценивания участниками мероприятия, проводимого в рамках конкурса мероприятий.
/// </summary>
public enum GradeMode
{
    /// <summary>
    /// Оценивать мероприятие могут только участники, зарегистрированные для участния в нём.
    /// </summary>
    Registered,

    /// <summary>
    /// Оценивать мероприятие могут любые участники, не имеющие прямого отношения к тому же отряду,
    /// что и само мероприятие.
    /// </summary>
    NonFriendly,

    /// <summary>
    /// Оценивать мероприятие могут любые участники.
    /// </summary>
    Friendly
}
