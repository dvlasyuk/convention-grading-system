namespace ConventionGradingSystem.Host.Components.Administration.Participants.Models;

public record class TeamsListModel(
    IReadOnlyCollection<TeamsListModel.TeamModel> Teams,
    string? SearchFilter)
{
    public string? SearchFilter { get; set; } = SearchFilter;

    public record TeamModel(
        int Number,
        string Identifier,
        string Name,
        int MembersQuantity,
        int RegistrationsQuantity,
        int AttendancesQuantity,
        int AbsencesQuantity,
        int SpecialMarksQuantity);
}
