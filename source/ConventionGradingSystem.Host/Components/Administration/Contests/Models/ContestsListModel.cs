namespace ConventionGradingSystem.Host.Components.Administration.Contests.Models;

public record class ContestsListModel
{
    public IEnumerable<ContestModel> Contests { get; set; } = [];
    public string? SearchFilter { get; set; }

    public record ContestModel(
        int Number,
        string Identifier,
        string Name,
        int EventsQuantity,
        int ExpertFeedbaksQuantity,
        int ParticiapntFeedbaksQuantity,
        string EventsListLink);
}
