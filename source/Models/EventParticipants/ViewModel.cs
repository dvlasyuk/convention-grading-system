namespace ConventionGradingSystem.Models.EventParticipants;

public record ViewModel(
    string EventTypeName,
    string EventName,
    IReadOnlyCollection<Participant> Participants);

public record Participant(
    string Identifier,
    string Name,
    string Brigade,
    string Team,
    bool ParticipitionMark,
    bool SpecialMark);
