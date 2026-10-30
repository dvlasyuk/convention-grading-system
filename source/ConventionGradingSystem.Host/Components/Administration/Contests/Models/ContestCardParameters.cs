namespace ConventionGradingSystem.Host.Components.Administration.Contests.Models;

public record ContestCardParameters(
    string ContestName,
    bool ExpertGrading,
    bool ParticipantGrading,
    bool RegisteredGrading,
    bool FriendlyGrading,
    bool AttendanceControl,
    IReadOnlyCollection<string> BannedTeams);
