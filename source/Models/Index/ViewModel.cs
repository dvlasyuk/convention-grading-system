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
    int ParticipantsQuantity,
    int ParticipationRegistrationsQuantity,
    int ParticipationMarksQuantity,
    int SpecialMarksQuantity);
