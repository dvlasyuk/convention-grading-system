namespace ConventionGradingSystem.Models.EventAttendanceForm;

/// <summary>
/// Информация о зарегистрированном на мероприятие участнике.
/// </summary>
/// <param name="Identifier">Идентификатор участника.</param>
/// <param name="Name">Полное имя участника.</param>
/// <param name="Brigade">Полное название отряда участника.</param>
/// <param name="AttendanceMark">Отметка о посещении участником мероприятия.</param>
/// <param name="SpecialMark">Специальная отметка участника от организаторов мероприятия.</param>
public record Participant(
    string Identifier,
    string Name,
    string Brigade,
    bool AttendanceMark,
    bool SpecialMark);
