namespace ConventionGradingSystem.DataAccess.Configuration.Models;

public class Contest
{
    public string Identifier { get; set; } = string.Empty;

    public string Name { get; set; } = "Неизвестный конкурс";

    public bool RegisteredGrading { get; set; } = true;

    public bool FriendlyGrading { get; set; } = true;

    public bool AttendanceControl { get; set; } = true;

    public ICollection<string> BannedTeams { get; } = [];

    public ICollection<GradeCriterion> ExpertCriterions { get; } = [];

    public ICollection<GradeCriterion> ParticipantCriterions { get; } = [];

    public ICollection<ContestEvent> Events { get; } = [];
}
