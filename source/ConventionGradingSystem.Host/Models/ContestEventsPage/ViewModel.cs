namespace ConventionGradingSystem.Host.Models.ContestEventsPage;

/// <summary>
/// Модель представления страницы приложения со списком мероприятий в рамках конкурса мероприятий.
/// </summary>
/// <param name="ContestName">Название конкурса мероприятий.</param>
/// <param name="ExpertCriterions">Критерии оценивания мероприятий экспертами.</param>
/// <param name="ParticipantCriterions">Критерии оценивания мероприятий участниками.</param>
/// <param name="Events">Информация о мероприятиях в рамках конкурса.</param>
public record ViewModel(
    string ContestName,
    IReadOnlyList<GradeCriterion> ExpertCriterions,
    IReadOnlyList<GradeCriterion> ParticipantCriterions,
    IReadOnlyList<ContestEvent> Events);
