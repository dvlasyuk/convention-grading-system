namespace ConventionGradingSystem.Host.Components.Administration.Contests.Models;

public record EventCardModel(
    string EventName,
    string ContestName,
    bool ParticipantsRegistered,
    IReadOnlyCollection<string> Brigades);
