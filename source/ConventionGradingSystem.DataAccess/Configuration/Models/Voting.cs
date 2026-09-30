namespace ConventionGradingSystem.DataAccess.Configuration.Models;

public class Voting
{
    public string Identifier { get; set; } = string.Empty;

    public string Name { get; set; } = "Неизвестное голосование";

    public bool BrigadeFriendlyVoting { get; set; } = true;

    public bool TeamFriendlyVoting { get; set; } = true;

    public int VotesQuantity { get; set; } = int.MinValue;

    public ICollection<string> BannedTeams { get; } = [];

    public ICollection<Candidate> Candidates { get; } = [];
}
