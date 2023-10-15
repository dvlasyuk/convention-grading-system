namespace ConventionGradingSystem.Models.ExpertNotes;

public record ViewModel(
    string EventTypeName,
    string EventName,
    IReadOnlyCollection<Note> Notes);

public record Note(
    int Identifier,
    string Content);
