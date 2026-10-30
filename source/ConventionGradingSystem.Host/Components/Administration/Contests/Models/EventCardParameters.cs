namespace ConventionGradingSystem.Host.Components.Administration.Contests.Models;

public record EventCardParameters(
    string EventName,
    string ContestName,
    bool ParticipantsRegistered,
    IReadOnlyCollection<string> Brigades);
