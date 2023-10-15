namespace ConventionGradingSystem.Models.Index;

public record ViewModel(
    IReadOnlyCollection<EventType> EventTypes,
    IReadOnlyCollection<Team> Teams);

public record EventType(
    int Identifier,
    string Name,
    int EventsQuantity);

public record Team(
    string Name,
    int ParticipantsQuantity,
    int ParticipationRegistrationsQuantity,
    int ParticipationMarksQuantity,
    int SpecialMarksQuantity);
