namespace ConventionGradingSystem.Models.EventParticipants;

public record FormModel(
    IReadOnlyCollection<string> ParticipationMarks,
    IReadOnlyCollection<string> SpecialMarks);
