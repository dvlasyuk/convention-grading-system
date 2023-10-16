namespace ConventionGradingSystem.Models.Index;

public record ViewModel(
    IReadOnlyList<Contest> Contests,
    IReadOnlyList<Team> Teams);

public record Contest(
    string Identifier,
    string Name,
    int EventsQuantity);

public record Team(
    string Name,
    int MembersQuantity,
    int MembersRegistrationsQuantity,
    int MembersMarksQuantity,
    int SpecialMarksQuantity);
