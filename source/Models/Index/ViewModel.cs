namespace ConventionGradingSystem.Models.Index;

public record ViewModel(
    IReadOnlyCollection<Contest> Contests,
    IReadOnlyCollection<Team> Teams);

public record Contest(
    int Identifier,
    string Name,
    int EventsQuantity);

public record Team(
    string Name,
    int ParticipantsQuantity,
    int ParticipationRegistrationsQuantity,
    int ParticipationMarksQuantity,
    int SpecialMarksQuantity);
