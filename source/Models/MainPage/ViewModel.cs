namespace ConventionGradingSystem.Models.MainPage;

public record ViewModel(
    IReadOnlyList<Contest> Contests,
    IReadOnlyList<Team> Teams);
