namespace ConventionGradingSystem.Host.Models.EventAttendanceForm;

/// <summary>
/// Модель данных формы приложения для сбора отметок о посещении мероприятия в рамках конкурса мероприятий.
/// </summary>
/// <param name="AttendanceMarks">Идентификаторы участников, для которых выставлены отметки о посещении.</param>
/// <param name="SpecialMarks">Идентификаторы участников, для которых выставлены специальные отметки.</param>
public record FormModel(
    IReadOnlyCollection<string>? AttendanceMarks,
    IReadOnlyCollection<string>? SpecialMarks);
