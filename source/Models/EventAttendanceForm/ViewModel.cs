namespace ConventionGradingSystem.Models.EventAttendanceForm;

/// <summary>
/// Модель представления формы приложения для сбора отметок о посещении мероприятия в рамках конкурса мероприятий.
/// </summary>
/// <param name="ContestName">Название конкурса мероприятий.</param>
/// <param name="EventName">Название мероприятия.</param>
/// <param name="Participants">Информация о зарегистрированных на мероприятие участниках.</param>
public record ViewModel(
    string ContestName,
    string EventName,
    IReadOnlyCollection<Participant> Participants);
