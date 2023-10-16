namespace ConventionGradingSystem.Models.EventParticipants;

public record ViewModel(
    string ContestName,
    string EventName,
    IReadOnlyCollection<Participant> Participants);

public record Participant(
    string Identifier,
    string Name,
    string Brigade,
    bool ParticipitionMark,
    bool SpecialMark);
