namespace ConventionGradingSystem.Models.MainPage;

/// <summary>
/// Информация о команде участников.
/// </summary>
/// <param name="Name">Человеко-читаемое название команды.</param>
/// <param name="MembersQuantity">Количество членов команды.</param>
/// <param name="RegistrationsQuantity">Количество регистраций членов команды на мероприятия в рамках
/// конкурсов мероприятий.</param>
/// <param name="AttendanceMarksQuantity">Количество отметок о посещении мероприятий в рамках конкурсов
/// мероприятий членами команды.</param>
/// <param name="SpecialMarksQuantity">Количество специальных отметок от организаторов мероприятий в
/// рамках конкурсов мероприятий, полученных членами команды.</param>
public record Team(
    string Name,
    int MembersQuantity,
    int RegistrationsQuantity,
    int AttendanceMarksQuantity,
    int SpecialMarksQuantity);
