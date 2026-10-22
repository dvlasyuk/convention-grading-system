namespace ConventionGradingSystem.Host.Components.Administration.Participants.Models;

public record class TeamsListModel
{
    public IEnumerable<TeamModel> Teams { get; set; } = [];
    public string? SearchFilter { get; set; }

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
